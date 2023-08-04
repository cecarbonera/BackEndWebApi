using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BackEndWebApi.Messageria
{
    public class Consumer
    {
        public Consumer() { }

        public void ConsumerMessage()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "order",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var data = channel.BasicGet("order", true);
                    var json = Encoding.UTF8.GetString(data.Body.ToArray());

                    var obj = JsonConvert.DeserializeObject<ObjetoPersonalisado>(json);

                    Console.WriteLine(obj.Mensagem);
                }
            }
        }
    }
}