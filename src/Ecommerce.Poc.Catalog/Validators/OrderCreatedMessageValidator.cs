using Ecommerce.Poc.Catalog.Dtos;
using FluentValidation;

namespace Ecommerce.Poc.Catalog.Validators
{
    public class OrderCreatedMessageValidator : AbstractValidator<OrderCreatedMessage>
    {
        public OrderCreatedMessageValidator()
        {
            RuleFor(x => x.OrderItems)
                .NotEmpty();
        }
    }
}