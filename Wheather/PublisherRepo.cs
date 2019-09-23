using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheather
{
    public class PublisherRepo
    {
        private readonly IModel _channel;
        private readonly RabbitMQConfig _rabbitMQConfig;

        public PublisherRepo(IConnection connection,
            RabbitMQConfig rabbitMQConfig)
        {
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(rabbitMQConfig.ExchangeName, ExchangeType.Fanout, true);
            _channel.BasicQos(0, 1, false);

            _rabbitMQConfig = rabbitMQConfig;
        }
        
        public void SendMessage(string cityName)
        {
            var correlationId = Guid.NewGuid().ToString();
            var args = new Dictionary<string, object>()
            {
                ["x-expires"] = 30000
            };
            var replyQueueName = _channel.QueueDeclare("",
                false,
                false,
                false,
                args).QueueName;
            var replyConsumer = new AsyncEventingBasicConsumer(_channel);
            replyConsumer.Received += async (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body);
                    await WriteToFile(JsonConvert.DeserializeObject<WeatherInfo>(response));
                    _channel.Dispose();
                }
            };

            _channel.BasicConsume(replyQueueName,   // Waiting for result from service
                true,
                replyConsumer);
            

            var body = Encoding.UTF8.GetBytes(cityName);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;

            _channel.BasicPublish(exchange: _rabbitMQConfig.ExchangeName,
                                 routingKey: "",
                                 basicProperties: properties,
                                 body: body);
        }

        private async Task WriteToFile(WeatherInfo info)
        {
            var infoText = $"{info.ServiceName}," +
                $"{info.City}," +
                $"{info.Date}," +
                $"{info.Humidity}," +
                $"{info.Pressure}," +
                $"{info.Temp}," +
                $"{info.WindSpeed}" +
                Environment.NewLine;

            if(!File.Exists("result.csv"))
            {
                infoText = $"ServiceName," +
                $"City," +
                $"Date," +
                $"Humidity," +
                $"Pressure," +
                $"Temp," +
                $"WindSpeed" +
                Environment.NewLine + 
                infoText;
            }

            await File.AppendAllTextAsync("result.csv", infoText);
        }
    }
}
