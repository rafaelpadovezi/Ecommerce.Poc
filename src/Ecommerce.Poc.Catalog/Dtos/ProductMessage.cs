using System;

namespace Ecommerce.Poc.Catalog.Dtos
{
    public record ProductCreatedMessage(Guid Id, string MaterialCode, string Name, string Description, int Stock);
}