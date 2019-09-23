using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQService.Models
{
    public class WeatherInfo
    {
        public string ServiceName { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public double Temp { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double WindSpeed { get; set; }
    }
}
