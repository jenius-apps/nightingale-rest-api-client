using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Helpers
{
    public class ResponseValueExtractor : IResponseValueExtractor
    {
        private readonly IJsonValueExtractor _jsonValueExtractor;
        private readonly IExpressionTreeBuilder _expressionTreeBuilder;

        public ResponseValueExtractor(
            IJsonValueExtractor jsonValueExtractor,
            IExpressionTreeBuilder expressionTreeBuilder)
        {
            _jsonValueExtractor = jsonValueExtractor ?? throw new ArgumentNullException(nameof(jsonValueExtractor));
            _expressionTreeBuilder = expressionTreeBuilder ?? throw new ArgumentNullException(nameof(expressionTreeBuilder));
        }

        public string Extract(WorkspaceResponse response, string propertyExpression)
        {
            if (response == null || string.IsNullOrWhiteSpace(propertyExpression))
            {
                return null;
            }

            Stack<string> expressionTree = _expressionTreeBuilder.ParseExpression(propertyExpression);

            if (expressionTree.Peek() == "response")
            {
                expressionTree.Pop();
            }

            string responseProperty = expressionTree.Count > 0 ? expressionTree.Pop() : string.Empty;

            switch (responseProperty)
            {
                case "Body":
                    return ExtractBodyValue(response, expressionTree);
                case "ContentType":
                    return response.ContentType;
                case "Headers":
                    return GetHeaderOrCookieValue(response.Headers, expressionTree.Pop());
                case "Cookies":
                    return GetHeaderOrCookieValue(response.Cookies, expressionTree.Pop());
                case "Successful":
                    return response.Successful.ToString().ToLower();
                case "StatusCode":
                    return response.StatusCode.ToString();
                case "StatusDescription":
                    return response.StatusDescription;
                case "TimeElapsed":
                    return response.TimeElapsed.ToString();
                default:
                    return null;
            }
        }

        private string ExtractBodyValue(WorkspaceResponse response, Stack<string> expressionTree)
        {
            if (response?.ContentType == null || expressionTree == null || expressionTree.Count == 0)
            {
                return null;
            }

            if (response.ContentType.Contains("application/json"))
            {
                return _jsonValueExtractor.GetValue(response.Body, expressionTree);
            }
            else if (response.ContentType.Contains("application/xml"))
            {
                // TODO add support for xml parsing
                return null;
            }
            else
            {
                return null;
            }
        }

        private string GetHeaderOrCookieValue(IList<KeyValuePair<string, string>> list, string key)
        {
            if (list == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            bool isInt = int.TryParse(key, out int index);

            if (isInt && index >= 0 && index < list.Count)
            {
                return list[index].Value;
            }
            else
            {
                return list.Where(x => x.Key.ToLower() == key.Trim('"').Trim('\'').ToLower()).Select(x => x.Value).FirstOrDefault();
            }
        }
    }
}
