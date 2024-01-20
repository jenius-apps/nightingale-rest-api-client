using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Interfaces
{
    public interface IResponseStatus
    {
        bool Successful { get; set; }

        int StatusCode { get; set; }

        string StatusDescription { get; set; }

        long TimeElapsed { get; set; }

        byte[] RawBytes { get; set; }

        bool? TestsAllPass { get; set; }
    }
}
