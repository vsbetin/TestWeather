using RabbitMQService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    public interface IWeatherRepo
    {
        Task<WeatherInfo> GetWeather(string name);
    }
}
