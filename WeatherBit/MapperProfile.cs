using AutoMapper;
using RabbitMQService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static WeatherBit.WeatherBitInfo;

namespace WeatherBit
{
    class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<DataInfo, WeatherInfo>()
                .ForMember(z => z.City, cfg => cfg.MapFrom(x => x.City_name))
                .ForMember(z => z.Humidity, cfg => cfg.MapFrom(x => x.Rh))
                .ForMember(z => z.Pressure, cfg => cfg.MapFrom(x => x.Pres))
                .ForMember(z => z.Date, cfg => cfg.MapFrom(x => DateTime.UtcNow))
                .ForMember(z => z.ServiceName, cfg => cfg.MapFrom(x => "WeatherBit"))
                .ForMember(z => z.Temp, cfg => cfg.MapFrom(x => x.Temp))
                .ForMember(z => z.WindSpeed, cfg => cfg.MapFrom(x => x.Wind_spd));
        }
    }
}
