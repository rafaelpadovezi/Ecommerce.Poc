using DotNetCore.CAP;

namespace DotNetCore.Cap.Idempotency
{
    public class ConsumerMessage<T>
    {
        public ConsumerMessage(CapHeader header, T body)
        {
            Header = header;
            Body = body;
        }
        
        public T Body { get; set; }
        private CapHeader Header { get; }

        public string Type
        {
            get
            {
                var hasCapMessageGroup = Header.TryGetValue("cap-msg-group", out var messageGroup);
                if (hasCapMessageGroup is false)
                {
                    // verify if another field can be used as Id
                }

                return messageGroup;
            }
        }

        public string MessageId
        {
            get
            {
                var hasCapId = Header.TryGetValue("cap-msg-id", out var messageId);
                if (hasCapId is false)
                {
                    // verify if another field can be used as Id
                }

                return messageId;
            }
        }

        public static ConsumerMessage<T> Create(CapHeader header, T body)
        {
            return new(header, body);
        }
    }

    public static class ConsumerMessage
    {
        public static ConsumerMessage<T> Create<T>(CapHeader header, T body)
        {
            return new(header, body);
        }
    }
}