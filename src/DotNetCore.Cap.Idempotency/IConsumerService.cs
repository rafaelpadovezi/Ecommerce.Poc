using System.Threading.Tasks;

namespace DotNetCore.Cap.Idempotency
{
    public interface IConsumerService<TMessage> 
    {
        Task ProcessMessageAsync(ConsumerMessage<TMessage> message);
    }
}