// Program.cs

using FlightChangeDetection.Data;
using FlightChangeDetection.Interfaces;
using FlightChangeDetection.Repository;
using FlightChangeDetection.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AirlineContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<AirlineContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IFlightRepository, FlightRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IFlightChangeDetector, FlightChangeDetector>();
        services.AddScoped<FlightChangeDetectionService>();
        
        
        
    })
    .Build();

var service = host.Services.GetRequiredService<FlightChangeDetectionService>();
service.Run(args);

await host.RunAsync();
