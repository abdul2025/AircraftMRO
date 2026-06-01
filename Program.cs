using AircraftMRO.Data;
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

// Logging Config
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
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
builder.Services.AddHangfireServer();


// MVC 
builder.Services.AddControllersWithViews();


// START New Registry of any services
builder.Services.AddSingleton<IAppLogger, CustomAppLogger>();
builder.Services.AddScoped<IAircraftService, AircraftService>();
// This to handle the generic assigning of model as per each initiate for the BaseRepo
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IAircraftStatusService, AircraftStatusService>();

builder.Services.AddScoped<AlertJobsService>();

// END New Registry of any services








var app = builder.Build();


// Register recurring jobs
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager =
        scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

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
    app.UseExceptionHandler("/Error");

    // Adds HTTP Strict Transport Security (HSTS)
    app.UseHsts();
}

// Handles status codes like 404, 403, etc.
app.UseStatusCodePagesWithReExecute("/Error/{0}");


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();



app.UseHangfireDashboard("/hangfire");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
