using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IJsonValueExtractor
    {
        string GetValue(string jsonString, Stack<string> expressionTree);
    }
}
