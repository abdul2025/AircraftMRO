using AircraftMRO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;
using SharedKernel.Logging.Infrastructure;
using Serilog;
using AircraftMRO.Services;
using AircraftMRO.Services.Interfaces;
using AircraftMRO.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Infrastructure.Identity.Seeders;
using Microsoft.AspNetCore.Authorization;
using AircraftMRO.Infrastructure.Hangfire;
using AircraftMRO.Infrastructure.Identity.Services.Interfaces;
using AircraftMRO.Infrastructure.Identity.Services;
using AircraftMRO.Infrastructure.BackgroundJobs;
using FluentValidation;
using FluentValidation.AspNetCore;
using AircraftMRO.Application.DTOs.Aircraft.Validators;
using AircraftMRO.Application.DTOs.MaintenanceRecord.Validators;
using AircraftMRO.Application.Services;
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Infrastructure.Hubs;
using Fluid;
using AircraftMRO.Application.DTOs.EmailTemplates;
using AircraftMRO.Application.DTOs.Emails.Settings;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Authorization;
using AircraftMRO.Common.Results;
using System.Text.Json;
using Scalar.AspNetCore;



// Logging Config
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/app.log")
    .CreateLogger();

// Application Configuration Builder
var builder = WebApplication.CreateBuilder(args);



// ********************************* //
// ******** Config START *********** //
// ********************************* //



// Serilog
builder.Host.UseSerilog();


// MediatR setup it will hook up all events in the for the same Program
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});



// DB Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
});
// Start Hangfire Worker
builder.Services.AddHangfireServer();


// Global mvc Authorization
builder.Services.AddControllersWithViews(options =>
{
    var mvcPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(mvcPolicy));
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
// For Aircraft
builder.Services.AddValidatorsFromAssemblyContaining<AircraftCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AircraftEditDtoValidator>();
// For Maintenance 
builder.Services.AddValidatorsFromAssemblyContaining<MaintenanceCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MaintenanceEditDtoValidator>();



builder.Services.AddSignalR();


// Saving Email Template in Fluid memory so no required of read from the server or hosting machine ever time looking for the X template
TemplateOptions.Default.MemberAccessStrategy.Register<OverdueWorkOrderEmailModel>();
TemplateOptions.Default.MemberAccessStrategy.Register<GroundedAircraftModel>();


// OpenApi Documentation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Aircraft MRO API";
        document.Info.Version = "v1";
        document.Info.Description = "Aircraft Maintenance Management System API";

        return Task.CompletedTask;
    });
});
// https://localhost:<port>/openapi/v1.json


// ********************************* //
// ******** Config END *********** //
// ********************************* //



// ********************************* //
// ***** Service Registry START ****** //
// ********************************* //


// This to handle the generic Log instance, controller , services ...etc.
builder.Services.AddScoped(typeof(IAppLogger<>), typeof(CustomAppLogger<>));
// This to handle the generic assigning of model as per each initiate for the BaseRepo
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Domain and Business 
builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IAircraftStatusService, AircraftStatusService>();

// Background Jobs
builder.Services.AddScoped<AlertJobsService>();

// Audit entity Globally 
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


// IDENTIFY Service 
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Notification
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<EmailTemplateService>();


// Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ProcessEmailNotifc>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


// ********************************* //
// ***** Service Registry END ****** //
// ********************************* //


// ********************************* //
// * IDENTITY // ERROR Handling START ** //
// ********************************* //

builder.Services.AddAuthentication(options =>
{
    // let the Authorize attributes decide in controller
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // TODO set expiration for the token
        ValidateIssuerSigningKey = true,

        // This reads Jwt:Issuer, Jwt:Audience, and Jwt:Key from configuration
        // Environment variable "Jwt__Issuer" maps to "Jwt:Issuer"
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing in configuration.")))
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = ServiceResult<string>.Failure("Unauthorized: You must provide a valid token.");
            var json = JsonSerializer.Serialize(result);

            return context.Response.WriteAsync(json);
        }
    };
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        // options.Password.RequiredUniqueChars = 4;
        // options.Password.RequireUppercase = true;
        // options.Password.RequireLowercase = true;
        // options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// config and path 
// config header and status for unauth and unauthenticated, where JS fetch will handle it.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Already Declared by ASP.NET Identity BUT keep it
    options.AccessDeniedPath = "/Error/403"; // For UnAuthorized Redirecting


    options.Events.OnRedirectToAccessDenied = context =>
    {
        // API routes — return JSON 403
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = ServiceResult<string>.Failure("Forbidden: You do not have permission.");
            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

        // AJAX fetch() from MVC pages (openCrudModal calls) — return plain 403 to be handle as toast notification 
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        // Full page navigation — redirect normally
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToLogin = context =>
    {
        // API routes — return JSON 401
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = ServiceResult<string>.Failure("Unauthorized: Authentication required.");
            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

        // AJAX fetch() from MVC pages — return plain 401 to be handle as toast notification 
        if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        // Full page navigation — redirect normally
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// ********************************* //
// * IDENTITY // ERROR Handling END * //
// ********************************* //




var app = builder.Build();

// ROLLS SEEDER
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
}


// Register recurring Hangfire jobs and their schedules.
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<AlertJobsService>(
        "alert-checks",
        service => service.RunAlertChecks(),
        Cron.MinuteInterval(30),
        new RecurringJobOptions()
    );

    recurringJobManager.AddOrUpdate<ProcessEmailNotifc>(
        "Process_Email_Notification",
        service => service.RunEmailProcessor(),
        Cron.MinuteInterval(30),
        new RecurringJobOptions()
    );
}

// Configure the HTTP request pipeline, PROD Setting
// if (!app.Environment.IsDevelopment())
// {
//     // Handles unhandled exceptions (500 errors)
//     app.UseExceptionHandler("/Error"); // it will look for ErrorController

//     // Adds HTTP Strict Transport Security (HSTS)
//     app.UseHsts();
// }

if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON endpoint
    app.MapOpenApi();

    // Interactive API UI
    app.MapScalarApiReference();

    // https://localhost:<port>/scalar/v1
}

// Handles status codes like that has no Body or content so it redirect to the ErrorController
app.UseStatusCodePagesWithReExecute("/Error/{0}"); // it will look for ErrorController




app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


// NotificationHub Routing
app.MapHub<NotificationHub>("/notificationHub");


// Routing for the hangfire and Authorization
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new HangfireAuthorizationFilter()
    ]
});

// API Routing
app.MapControllers();


// MVC Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();


