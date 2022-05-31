using FluentValidation;
using System;
using System.Threading.Tasks;
using Ziggurat;

namespace Ecommerce.Poc.Catalog.Infrastructure.ConsumerMiddlewares
{
    public class ValidationMiddleware<TMessage> : IConsumerMiddleware<TMessage>
        where TMessage : IMessage
    {
        private readonly IValidatorFactory _validatorFactory;

        public ValidationMiddleware(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }
        
        public async Task OnExecutingAsync(TMessage message, ConsumerServiceDelegate<TMessage> next)
        {
            var validator = _validatorFactory.GetValidator<TMessage>();
            var context = new ValidationContext<TMessage>(message);
            var result = await validator.ValidateAsync(context);

            if (!result.IsValid)
                throw new Exception("Message is not valid");
            
            await next(message);
        }
    }
}