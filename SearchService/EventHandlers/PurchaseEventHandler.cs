using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SearchService.Models.EntityModels;
using SearchService.Services;

namespace SearchService.EventHandlers;

public class PurchaseEventHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PurchaseEventHandler> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public PurchaseEventHandler(IServiceProvider serviceProvider, ILogger<PurchaseEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _connection = Infrastructure.RabbitMqConfiguration.CreateConnection(configuration);
        _channel = _connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PurchaseEventHandler starting...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var purchase = JsonSerializer.Deserialize<PurchaseEntity>(message);

                if (purchase != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
                    
                    var success = await indexingService.IndexPurchaseAsync(purchase);
                    
                    if (success)
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation("Successfully processed purchase {PurchaseId}", purchase.PurchaseId);
                    }
                    else
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                        _logger.LogWarning("Failed to process purchase {PurchaseId}, requeuing", purchase.PurchaseId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing purchase event");
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(queue: "purchases-queue", autoAck: false, consumer: consumer);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
