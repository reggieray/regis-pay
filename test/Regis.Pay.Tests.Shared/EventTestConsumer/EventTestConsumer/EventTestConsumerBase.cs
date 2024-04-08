using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Regis.Pay.Domain;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.EventTestConsumer
{
    public abstract class EventTestConsumerBase<T> where T : class, IIntegrationEvent
    {
        private T _event = default!;
        public abstract string ExchangeName { get; }

        public async Task<T> ListenToEvent(Func<Task> function, int waitMS = 30000)
        {
            var tokenSource = new CancellationTokenSource();

            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                  exchange: ExchangeName,
                  routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body) as dynamic;

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var consumedMessage = JsonSerializer.Deserialize<ConsumedMessage>(message, jsonOptions);

                var msgAsString = JsonSerializer.Serialize(consumedMessage.Message);

                _event = JsonSerializer.Deserialize<T>(msgAsString, jsonOptions);

                tokenSource.Cancel();
                tokenSource.Dispose();
            };

            channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

            await function();

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(waitMS), tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                //swallowing expected exeption
            }

            return _event;
        }
    }
}
