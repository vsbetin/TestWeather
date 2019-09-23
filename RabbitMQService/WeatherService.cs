using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQService.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQService
{
    // Hosted service for weather services
    public class WeatherService : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IWeatherRepo _weatherRepo;
        private readonly RabbitMQConfig _rabbitMQConfig;

        public WeatherService(IWeatherRepo weatherRepo,
            IModel channel,
            RabbitMQConfig rabbitMQConfig)
        {
            _weatherRepo = weatherRepo;
            _channel = channel;
            _rabbitMQConfig = rabbitMQConfig;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var cityName = System.Text.Encoding.UTF8.GetString(ea.Body);

                var weatherInfo = await _weatherRepo.GetWeather(cityName);
                if (weatherInfo != null)
                {
                    var jsonWeatherInfo = JsonConvert.SerializeObject(weatherInfo);
                    Console.WriteLine(jsonWeatherInfo);

                    var props = ea.BasicProperties;
                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;
                    
                    var response = Encoding.UTF8.GetBytes(jsonWeatherInfo);
                    _channel.BasicPublish(exchange: "", 
                        routingKey: props.ReplyTo,
                        basicProperties: replyProps, 
                        body: response);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_rabbitMQConfig.QueueName, false, consumer);
            return Task.CompletedTask;
        }
    }
}
