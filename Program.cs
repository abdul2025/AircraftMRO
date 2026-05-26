using AircraftMRO.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;
using SharedKernel.Logging.Infrastructure;
using Serilog;
using AircraftMRO.Services;
using AircraftMRO.Services.Interfaces;

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
// MVC 
builder.Services.AddControllersWithViews();


// START New Registry of any services
builder.Services.AddSingleton<IAppLogger, CustomAppLogger>();
builder.Services.AddScoped<IAircraftService, AircraftService>();
// END New Registry of any services








var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
