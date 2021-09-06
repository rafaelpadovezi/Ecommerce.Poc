using System.Threading.Tasks;
using DotNetCore.CAP;
using Ecommerce.Poc.Search.Dtos;
using Ecommerce.Poc.Search.Models;
using Microsoft.Extensions.Logging;
using Nest;

namespace Ecommerce.Poc.Search.Consumers
{
    public class ProductCreatedConsumer : ICapSubscribe
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductCreatedConsumer> _logger;

        public ProductCreatedConsumer(
            IElasticClient elasticClient,
            ILogger<ProductCreatedConsumer> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        [CapSubscribe("product.created")]
        public async Task AddProductAsync(ProductCreatedMessage message)
        {
            var product = new Product
            {
                Id = message.Id,
                MaterialCode = message.MaterialCode,
                Name = message.Name,
                Description = message.Description
            };

            var response = await _elasticClient.IndexDocumentAsync(product);

            if (response.IsValid)
                _logger.LogInformation("Product added.");
            else
                _logger.LogWarning($"Product not added: {response.ServerError}");
        }
    }
}