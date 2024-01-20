using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Helpers
{
    public interface IMessageBus
    {
        event EventHandler<MessageArgs> MessagePublished;

        void Publish(IMessage message);
    }
}
