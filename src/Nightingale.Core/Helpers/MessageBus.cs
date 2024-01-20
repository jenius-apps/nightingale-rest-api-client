using System;

namespace Nightingale.Core.Helpers
{
    public class MessageBus : IMessageBus
    {
        public event EventHandler<MessageArgs> MessagePublished;

        public void Publish(IMessage message)
        {
            MessagePublished?.Invoke(this, new MessageArgs(message));
        }
    }
}
