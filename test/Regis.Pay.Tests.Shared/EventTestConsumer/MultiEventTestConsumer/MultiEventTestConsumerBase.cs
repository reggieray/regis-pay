using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Regis.Pay.Domain;
using System.Collections.Concurrent;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.MultiEventTestConsumer
{
    public abstract class MultiEventTestConsumerBase<T> : IDisposable where T : class, IIntegrationEvent
    {
        private readonly IModel _channel;

        public readonly ConcurrentBag<string> EventIds = new();
        public abstract string ExchangeName { get; }

        public abstract void Add(T @event);

        protected MultiEventTestConsumerBase()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public void ListenToEvents()
        {
            var queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queueName,
                  exchange: ExchangeName,
                  routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(_channel);
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

                var @event = JsonSerializer.Deserialize<T>(msgAsString, jsonOptions);

                Add(@event);
            };

            _channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _channel.Dispose();
        }
    }
}
