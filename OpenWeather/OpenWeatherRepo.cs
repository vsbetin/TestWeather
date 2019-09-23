using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQService;
using RabbitMQService.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeather
{
    class OpenWeatherRepo : IWeatherRepo
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly IMapper _mapper;

        public OpenWeatherRepo(IConfiguration configuration,
            IMapper mapper)
        {
            _mapper = mapper;
            var apiInfo = configuration.GetSection("OpenWeatherAPI");
            _apiKey = apiInfo.GetValue<string>("APIKey");
            _client = new HttpClient
            {
                BaseAddress = new Uri(apiInfo.GetValue<string>("Endpoint"))
            };
        }

        public async Task<WeatherInfo> GetWeather(string name)
        {
            var result = await _client.GetAsync($"weather?q={name}&APPID={_apiKey}");

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var openWeatherInfo = JsonConvert.DeserializeObject<OpenWeatherInfo>(await result.Content.ReadAsStringAsync());
                return _mapper.Map<WeatherInfo>(openWeatherInfo);
            }

            return null;
        }
    }

    class OpenWeatherInfo
    {
        public string Name { get; set; }
        public MainInfo Main { get; set; }
        public WindInfo Wind { get; set; }

        internal class MainInfo
        {
            public double Temp { get; set; }
            public double Pressure { get; set; }
            public double Humidity { get; set; }
        }

        internal class WindInfo
        {
            public double Speed { get; set; }
        }
    }

}
