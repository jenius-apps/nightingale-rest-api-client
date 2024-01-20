using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Interfaces
{
    public interface IParameter : IStorageItem
    {
        string Key { get; set; }

        string Value { get; set; }

        bool Enabled { get; set; }
    }
}
