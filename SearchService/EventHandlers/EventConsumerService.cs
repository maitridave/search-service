using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SearchService.Models.Events;
using SearchService.Services;

namespace SearchService.EventHandlers;

public class EventConsumerService : BackgroundService
{
    private readonly ILogger<EventConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    
    public EventConsumerService(
        ILogger<EventConsumerService> logger, 
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitializeRabbitMQ();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                
                _logger.LogInformation("Received message with routing key: {RoutingKey}", routingKey);
                
                await ProcessMessage(message, routingKey);
                
                _channel?.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        // Subscribe to all automotive-related queues
        _channel?.BasicConsume(queue: "automotive.offers", autoAck: false, consumer: consumer);
        _channel?.BasicConsume(queue: "automotive.purchases", autoAck: false, consumer: consumer);
        _channel?.BasicConsume(queue: "automotive.transports", autoAck: false, consumer: consumer);

        _logger.LogInformation("Event consumer started and listening for messages...");

        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task InitializeRabbitMQ()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "rabbitmq",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(exchange: "automotive.events", type: ExchangeType.Topic, durable: true);

            // Declare queues
            _channel.QueueDeclare(queue: "automotive.offers", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "automotive.purchases", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "automotive.transports", durable: true, exclusive: false, autoDelete: false);

            // Bind queues to exchange
            _channel.QueueBind(queue: "automotive.offers", exchange: "automotive.events", routingKey: "offer.*");
            _channel.QueueBind(queue: "automotive.purchases", exchange: "automotive.events", routingKey: "purchase.*");
            _channel.QueueBind(queue: "automotive.transports", exchange: "automotive.events", routingKey: "transport.*");

            _logger.LogInformation("RabbitMQ connection established and queues configured");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    private async Task ProcessMessage(string message, string routingKey)
    {
        using var scope = _serviceProvider.CreateScope();
        var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();

        try
        {
            if (routingKey.StartsWith("offer."))
            {
                var offerEvent = JsonSerializer.Deserialize<OfferEvent>(message);
                if (offerEvent != null)
                {
                    await indexingService.ProcessOfferEvent(offerEvent);
                }
            }
            else if (routingKey.StartsWith("purchase."))
            {
                var purchaseEvent = JsonSerializer.Deserialize<PurchaseEvent>(message);
                if (purchaseEvent != null)
                {
                    await indexingService.ProcessPurchaseEvent(purchaseEvent);
                }
            }
            else if (routingKey.StartsWith("transport."))
            {
                var transportEvent = JsonSerializer.Deserialize<TransportEvent>(message);
                if (transportEvent != null)
                {
                    await indexingService.ProcessTransportEvent(transportEvent);
                }
            }
            else
            {
                _logger.LogWarning("Unknown routing key: {RoutingKey}", routingKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message for routing key: {RoutingKey}", routingKey);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
