using System.Threading.Tasks;

namespace DotNetCore.Cap.Idempotency
{
    public interface IConsumerService<TMessage> where TMessage : IMessage
    {
        Task ProcessMessageAsync(TMessage message);
    }
}