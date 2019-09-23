using AutoMapper;
using RabbitMQService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeather
{
    class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<OpenWeatherInfo, WeatherInfo>()
                .ForMember(z => z.City, cfg => cfg.MapFrom(x => x.Name))
                .ForMember(z => z.Humidity, cfg => cfg.MapFrom(x => x.Main.Humidity))
                .ForMember(z => z.Pressure, cfg => cfg.MapFrom(x => x.Main.Pressure))
                .ForMember(z => z.Date, cfg => cfg.MapFrom(x => DateTime.UtcNow))
                .ForMember(z => z.ServiceName, cfg => cfg.MapFrom(x => "OpenWeather"))
                .ForMember(z => z.Temp, cfg => cfg.MapFrom(x => x.Main.Temp))
                .ForMember(z => z.WindSpeed, cfg => cfg.MapFrom(x => x.Wind.Speed));
        }
    }
}
