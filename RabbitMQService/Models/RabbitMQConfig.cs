using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQService.Models
{
    public class RabbitMQConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
    }
}
