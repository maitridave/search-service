using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SearchService.Models.EntityModels;
using SearchService.Services;

namespace SearchService.EventHandlers;

public class TransportEventHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TransportEventHandler> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public TransportEventHandler(IServiceProvider serviceProvider, ILogger<TransportEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        _connection = Infrastructure.RabbitMqConfiguration.CreateConnection(configuration);
        _channel = _connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TransportEventHandler starting...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var transport = JsonSerializer.Deserialize<TransportEntity>(message);

                if (transport != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
                    
                    var success = await indexingService.IndexTransportAsync(transport);
                    
                    if (success)
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation("Successfully processed transport {TransportId}", transport.TransportId);
                    }
                    else
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                        _logger.LogWarning("Failed to process transport {TransportId}, requeuing", transport.TransportId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transport event");
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(queue: "transports-queue", autoAck: false, consumer: consumer);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
