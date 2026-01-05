using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SearchService.Models.Events;

namespace SearchService.EventHandlers;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, string routingKey) where T : BaseEvent;
}

public class EventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IConfiguration configuration, ILogger<EventPublisher> logger)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:Host"] ?? "rabbitmq",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Ensure exchange exists
        _channel.ExchangeDeclare(exchange: "automotive.events", type: ExchangeType.Topic, durable: true);
    }

    public async Task PublishAsync<T>(T eventData, string routingKey) where T : BaseEvent
    {
        try
        {
            var message = JsonSerializer.Serialize(eventData);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "automotive.events",
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );

            _logger.LogInformation("Published event {EventType} with routing key {RoutingKey}", 
                eventData.EventType, routingKey);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} with routing key {RoutingKey}", 
                eventData.EventType, routingKey);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
