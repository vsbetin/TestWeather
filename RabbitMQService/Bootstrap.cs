using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using RabbitMQService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQService
{
    public static class Bootstrap
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, RabbitMQConfig rabbitConfig)
        {
            services.TryAddSingleton<RabbitMQConfig>(rabbitConfig);

            services.TryAddSingleton<IConnection>(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<RabbitMQConfig>();
                var factory = new ConnectionFactory()
                {
                    HostName = config.Host,
                    Port = config.Port,
                    DispatchConsumersAsync = true
                };
                return factory.CreateConnection();
            });

            services.TryAddTransient<IModel>(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<RabbitMQConfig>();
                var connection = serviceProvider.GetRequiredService<IConnection>();
                var channel = connection.CreateModel();

                channel.ExchangeDeclare(config.ExchangeName, ExchangeType.Fanout, true);
                channel.QueueDeclare(config.QueueName, true, false, false, null);
                channel.QueueBind(config.QueueName, config.ExchangeName, "", null);
                channel.BasicQos(0, 1, false);

                return channel;
            });

            return services;
        }
    }
}
