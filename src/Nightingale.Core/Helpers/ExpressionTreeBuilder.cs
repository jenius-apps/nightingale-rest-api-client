using Nightingale.Core.Helpers.Interfaces;
using System;
using System.Collections.Generic;

namespace Nightingale.Core.Helpers
{
    public class ExpressionTreeBuilder : IExpressionTreeBuilder
    {
        public Stack<string> ParseExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return new Stack<string>();
            }

            Stack<string> result = new Stack<string>();
            string[] terms = expression.Split('.');

            foreach (string term in terms)
            {
                // if looks like propertyName[3] or propertyName["sdkjf"]
                if (term.Contains("[") && term.Contains("]") && term.IndexOf("[") < term.IndexOf("]"))
                {
                    string[] splitTerms = term.Split('[');

                    if (splitTerms.Length != 2)
                    {
                        throw new InvalidOperationException("Unsure how to parse " + term);
                    }

                    result.Push(splitTerms[0]);
                    result.Push(splitTerms[1].Trim(']'));
                }
                else
                {
                    result.Push(term);
                }
            }

            Stack<string> reversed = new Stack<string>();

            while (result.Count > 0)
            {
                reversed.Push(result.Pop());
            }

            return reversed;
        }
    }
}
