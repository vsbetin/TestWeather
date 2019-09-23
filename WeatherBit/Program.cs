using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQService;
using RabbitMQService.Models;
using System;
using System.Reflection;

namespace WeatherBit
{
    class Program
    {
        static void Main(string[] args)
        {
            new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                    config.AddJsonFile("appsettings.json", false, true)
                )
                .UseConsoleLifetime()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddTransient<IWeatherRepo, WeatherBitRepo>()
                        .AddAutoMapper(Assembly.GetExecutingAssembly())
                        .AddRabbitMQ(hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>())
                        .AddHostedService<WeatherService>();
                })
                .Build()
                .Run();
        }
    }
}
