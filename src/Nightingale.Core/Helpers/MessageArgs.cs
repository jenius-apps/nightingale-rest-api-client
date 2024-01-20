using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Helpers
{
    public class MessageArgs : EventArgs
    {
        public IMessage Message { get; set; }

        public MessageArgs(IMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
