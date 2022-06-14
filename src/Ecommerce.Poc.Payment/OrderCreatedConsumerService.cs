using MongoDB.Driver;
using Ziggurat;
using Ziggurat.MongoDB;

namespace Ecommerce.Poc.Payment;

public class OrderCreatedConsumerService : IConsumerService<OrderCreatedMessage>
{
    private readonly IMongoClient _client;
    private readonly ILogger<OrderCreatedConsumerService> _logger;

    public OrderCreatedConsumerService(ILogger<OrderCreatedConsumerService> logger, IMongoClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task ProcessMessageAsync(OrderCreatedMessage message)
    {
        var databaseName = "payment";
        _logger.LogInformation(message.ToString());

        using var session = _client.StartIdempotentTransaction(message);
        var collection = _client.GetDatabase(databaseName).GetCollection<OrderCreatedMessage>("orders");
        await collection.InsertOneAsync(session, message);
    }
}