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

namespace WeatherBit
{
    class WeatherBitRepo : IWeatherRepo
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly IMapper _mapper;

        public WeatherBitRepo(IConfiguration configuration,
            IMapper mapper)
        {
            _mapper = mapper;
            var apiInfo = configuration.GetSection("WeatherBitAPI");
            _apiKey = apiInfo.GetValue<string>("APIKey");
            _client = new HttpClient
            {
                BaseAddress = new Uri(apiInfo.GetValue<string>("Endpoint"))
            };
        }

        public async Task<WeatherInfo> GetWeather(string name)
        {
            var result = await _client.GetAsync($"current?city={name}&key={_apiKey}");

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var weatherBitInfo = JsonConvert.DeserializeObject<WeatherBitInfo>(await result.Content.ReadAsStringAsync());
                if (weatherBitInfo.Data != null && weatherBitInfo.Data.Length > 0)
                {
                    return _mapper.Map<WeatherInfo>(weatherBitInfo.Data[0]);
                }
            }

            return null;
        }
    }

    class WeatherBitInfo
    {
        public DataInfo[] Data { get; set; }

        internal class DataInfo
        {
            public string City_name { get; set; }
            public double Temp { get; set; }
            public double Pres { get; set; }
            public double Rh { get; set; }
            public double Wind_spd { get; set; }
        }
    }

}
