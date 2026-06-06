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
using AircraftMRO.BackgroundJobs;
using Microsoft.AspNetCore.Identity;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Infrastructure.Identity.Seeders;
using Microsoft.AspNetCore.Authorization;
using AircraftMRO.Infrastructure.Hangfire;
using AircraftMRO.Infrastructure.Identity.Services.Interfaces;
using AircraftMRO.Infrastructure.Identity.Services;

// Logging Config
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate:"[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/app.log")
    .CreateLogger();

// Application Configuration Builder
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();






// DB Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection")));
});
// Start Worker
builder.Services.AddHangfireServer();


// MVC 
builder.Services.AddControllersWithViews();


// START New Registry of any services
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
builder.Services.AddScoped<IIdentityService, IdentityService>();

// END New Registry of any services



// IDENTITY
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

// Redirection for unauth and unauthenticated 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect for if not logged in
    options.AccessDeniedPath = "/Error/403"; // Redirect for lacks permission TODO can Create a dedicated AccessDenied PAGE
});
// Global Authorization
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});



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
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Handles unhandled exceptions (500 errors)
    app.UseExceptionHandler("/Error"); // it will look for ErrorController

    // Adds HTTP Strict Transport Security (HSTS)
    app.UseHsts();
}

// Handles status codes like 404, 403, etc.
app.UseStatusCodePagesWithReExecute("/Error/{0}"); // it will look for ErrorController




app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Routing for the hangfire and Authorization
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new HangfireAuthorizationFilter()
    ]
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
