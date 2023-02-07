using DotNetCore.CAP;
using DotNetCore.CAP.RabbitMQ;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.Options;

namespace Ecommerce.Poc.Catalog;

public class CustomRabbitMQConsumerClientFactory : IConsumerClientFactory
{
    private readonly IConnectionChannelPool _connectionChannelPool;
    private readonly IOptions<RabbitMQOptions> _rabbitMQOptions;

    public CustomRabbitMQConsumerClientFactory(IOptions<RabbitMQOptions> rabbitMQOptions, IConnectionChannelPool channelPool)
    {
        _rabbitMQOptions = rabbitMQOptions;
        _connectionChannelPool = channelPool;
    }

    public IConsumerClient Create(string groupId)
    {
        try
        {
            var client = new CustomRabbitMQConsumerClient(groupId, _connectionChannelPool, _rabbitMQOptions);
            client.Connect();
            return client;
        }
        catch (System.Exception e)
        {
            throw new BrokerConnectionException(e);
        }
    }
}