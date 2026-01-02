using RabbitMQ.Client;

namespace SearchService.Infrastructure;

public class RabbitMqConfiguration
{
    public static IConnection CreateConnection(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        return factory.CreateConnection();
    }

    public static void DeclareQueues(IModel channel)
    {
        // Declare queues for each entity type
        channel.QueueDeclare(
            queue: "offers-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        channel.QueueDeclare(
            queue: "purchases-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        channel.QueueDeclare(
            queue: "transports-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Declare dead letter queues
        channel.QueueDeclare(
            queue: "dead-letter-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }
}
