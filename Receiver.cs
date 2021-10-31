using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application
{
    public class Receiver
    {
        private string _hostName;
        private string _queueName;
        
        public Receiver(string hostName, string queueName)
        {
            _hostName = hostName;
            _queueName = queueName;
        }
        
        public List<String> GetMessages()
        {
            var messages = new List<string>();

            var factory = new ConnectionFactory() { HostName = _hostName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        var log = JsonConvert.DeserializeObject<string>(message);
                        messages.Add(log);

                        Console.WriteLine(message);
                    };

                    channel.BasicConsume(queue: _queueName,
                        consumer: consumer);
                }
            }
            return messages;
        }
    }
}