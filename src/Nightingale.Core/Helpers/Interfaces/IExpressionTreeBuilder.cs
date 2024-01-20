using System.Collections.Generic;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IExpressionTreeBuilder
    {
        Stack<string> ParseExpression(string expression);
    }
}
