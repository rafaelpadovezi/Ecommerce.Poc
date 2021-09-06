using System;

namespace Ecommerce.Poc.Search.Dtos
{
    public record ProductCreatedMessage(Guid Id, string MaterialCode, string Name, string Description, int Stock);
}