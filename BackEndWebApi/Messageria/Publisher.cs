using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace BackEndWebApi.Messageria
{
    public class Publisher
    {
        public Publisher() { }

        public void PublisherMessage(ObjetoPersonalisado obj)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ConfirmSelect();
                channel.QueueDeclare(queue: "order",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "",
                                     routingKey: "order",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine("Mensagem enviada!");
            }
        }
    }
}
