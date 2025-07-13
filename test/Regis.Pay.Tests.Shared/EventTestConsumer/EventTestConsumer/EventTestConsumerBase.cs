using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Regis.Pay.Domain;

namespace Regis.Pay.Tests.Shared.EventTestConsumer.EventTestConsumer;

public abstract class EventTestConsumerBase<T> where T : class, IIntegrationEvent
{
    private T _event = null!;
    protected abstract string ExchangeName { get; }

    public async Task<T> ListenToEvent(Func<Task> function, string rabbitmq, int waitMs = 30000)
    {
        var tokenSource = new CancellationTokenSource();

        var factory = new ConnectionFactory { Uri = new Uri(rabbitmq) };
        await using var connection = await factory.CreateConnectionAsync(tokenSource.Token);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: tokenSource.Token);

        var queue = await channel.QueueDeclareAsync(cancellationToken: tokenSource.Token);
            
        await channel.QueueBindAsync(queue: queue.QueueName,
            exchange: ExchangeName,
            routingKey: string.Empty, cancellationToken: tokenSource.Token);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
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

            await tokenSource.CancelAsync();
            tokenSource.Dispose();
        };

        await channel.BasicConsumeAsync(queue: queue.QueueName,
            autoAck: true,
            consumer: consumer, cancellationToken: tokenSource.Token);

        await function();

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(waitMs), tokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            //swallowing expected exception
        }

        return _event;
    }
}