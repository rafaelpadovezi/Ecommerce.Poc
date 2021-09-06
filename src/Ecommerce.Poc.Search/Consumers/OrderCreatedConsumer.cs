using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Ecommerce.Poc.Search.Dtos;
using Ecommerce.Poc.Search.Models;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;

namespace Ecommerce.Poc.Search.Consumers
{
    public class OrderCreatedConsumer : ICapSubscribe
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public OrderCreatedConsumer(
            IElasticClient elasticClient,
            ILogger<ProductCreatedConsumer> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        [CapSubscribe("order.created")]
        public async Task UpdateTotalSalesAsync(OrderCreatedMessage message)
        {
            var materialCodes = message.OrderItems.Select(x => x.MaterialCode);

            var response = await IncrementMaterialsTotalSalesAsync(materialCodes);

            if (response.IsValid)
                _logger.LogInformation("Products updated.");
            else
                _logger.LogWarning($"Products not updated: {response.ServerError}");
        }

        private async Task<UpdateByQueryResponse> IncrementMaterialsTotalSalesAsync(IEnumerable<string> materialCodes)
        {
            var response = await _elasticClient.UpdateByQueryAsync<Product>(u => u.Query(q =>
                {
                    QueryContainer producerQuery = null;
                    foreach (var materialCode in materialCodes)
                    {
                        producerQuery = producerQuery || q.MatchPhrase(m => m
                            .Field(f => f.MaterialCode)
                            .Query(materialCode)
                        );
                    }

                    return producerQuery;
                })
                .Script("ctx._source.totalSales++")
                .Conflicts(Conflicts.Proceed));
            return response;
        }
    }
}