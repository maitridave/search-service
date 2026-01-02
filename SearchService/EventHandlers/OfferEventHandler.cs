using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SearchService.Models.EntityModels;
using SearchService.Services;

namespace SearchService.EventHandlers;

public class OfferEventHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OfferEventHandler> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public OfferEventHandler(IServiceProvider serviceProvider, ILogger<OfferEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Note: In production, inject IConnection from DI
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _connection = Infrastructure.RabbitMqConfiguration.CreateConnection(configuration);
        _channel = _connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OfferEventHandler starting...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var offer = JsonSerializer.Deserialize<OfferEntity>(message);

                if (offer != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
                    
                    var success = await indexingService.IndexOfferAsync(offer);
                    
                    if (success)
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation("Successfully processed offer {OfferId}", offer.OfferId);
                    }
                    else
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                        _logger.LogWarning("Failed to process offer {OfferId}, requeuing", offer.OfferId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing offer event");
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(queue: "offers-queue", autoAck: false, consumer: consumer);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
