using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WaterCoolerApp
{
    class WaterCoolerApp
    {
        private static readonly string RabbitMqUrl = "amqps://fhshurll:N3mUUPbhrsa4wIHpJILagc9DZeeYjObD@cow.rmq2.cloudamqp.com/fhshurll";
        private const string ExchangeName = "water_cooler_exchange";
        private const string RoutingKey = "es.aa123.#";
        private const string ServerUrl = "http://localhost:8011/water_coolers/1"; // Modify the ID as needed
        private static readonly HttpClient httpClient = new HttpClient();

        public void Start()
        {
            // Connect to RabbitMQ
            ConnectToRabbitMq();
        }

        private void ConnectToRabbitMq()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(RabbitMqUrl),
                DispatchConsumersAsync = true
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the exchange and temporary queue
            channel.ExchangeDeclare(exchange: ExchangeName, type: "topic", durable: true);
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: RoutingKey);

            Console.WriteLine("Connected to RabbitMQ and listening for messages...");

            // Listener for incoming messages
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");

                try
                {
                    // Process the message and send it to the server
                    await SendDataToServerAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            // Keep the app running to continue receiving messages
            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }

        private async Task SendDataToServerAsync(string message)
        {
            try
            {
                // Send data to the server
                var content = new StringContent(message, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(ServerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent to server successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send data to server. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to server: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new WaterCoolerApp();
            app.Start();
        }
    }
}
