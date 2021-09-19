using System;
using System.Text.Json;
using DotNetCore.CAP.Filter;
using DotNetCore.CAP.Messages;
using Ecommerce.Poc.Catalog.Dtos;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Poc.Catalog.Infrastructure.CapFilters
{
    public class LoggingFilter : SubscribeFilter
    {
        private readonly ILogger<LoggingFilter> _logger;

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            _logger = logger;
        }

        // before subscribe method exectuing
        public override void OnSubscribeExecuting(ExecutingContext context)
        {
            _logger.LogDebug("Message received from customer {message}", context.DeliverMessage.Value);
        }
    }
}