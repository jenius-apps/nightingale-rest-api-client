using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Helpers
{
    public interface IMessage
    {
        string StringContent { get; set; }

        string UriContent { get; set; }

        bool IsClickable { get; set; }
    }
}
