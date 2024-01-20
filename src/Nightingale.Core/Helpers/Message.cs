using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Helpers
{
    public class Message : IMessage
    {
        public Message(string stringContent = null, string uri = null)
        {
            StringContent = stringContent;
            UriContent = uri;
        }

        public string StringContent { get; set; }

        public string UriContent { get; set; }

        public bool IsClickable { get; set; }
    }
}
