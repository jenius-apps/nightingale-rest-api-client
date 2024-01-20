using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nightingale.Core.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Helpers
{
    public class JsonValueExtractor : IJsonValueExtractor
    {
        public string GetValue(string jsonString, Stack<string> expressionTree)
        {
            if (string.IsNullOrWhiteSpace(jsonString) || expressionTree == null || expressionTree.Count == 0)
            {
                return null;
            }

            JContainer json;
            string result;

            try
            {
                if (jsonString.StartsWith("["))
                {
                    json = JArray.Parse(jsonString);
                    result = ParseJson((JArray)json, expressionTree);
                }
                else
                {
                    json = JObject.Parse(jsonString);
                    result = ParseJson((JObject)json, expressionTree);
                }
            }
            catch (JsonReaderException)
            {
                result = null;
            }

            return result;
        }

        private string ParseJson(JArray current, Stack<string> expressionTree)
        {
            if (expressionTree == null || current == null)
            {
                return null;
            }

            if (expressionTree.Count == 0)
            {
                return current.ToString();
            }
            else
            {
                string term = expressionTree.Pop();
                bool isInt = int.TryParse(term, out int index);

                if (isInt && (index < 0 || index >= current.Count))
                {
                    return null;
                }

                JToken next = isInt ? current[index] : current[term];

                if (next == null)
                {
                    return null;
                }
                if (next.Type == JTokenType.Array)
                {
                    return ParseJson((JArray)next, expressionTree);
                }
                else
                {
                    return ParseJson((JObject)next, expressionTree);
                }
            }
        }

        private string ParseJson(JObject current, Stack<string> expressionTree)
        {
            if (expressionTree == null || current == null)
            {
                return null;
            }

            if (expressionTree.Count == 0)
            {
                return current.ToString();
            }
            else
            {
                string term = expressionTree.Pop();
                JToken next = current[term];

                if (next == null)
                {
                    return null;
                }
                else if (next.Type == JTokenType.Array)
                {
                    return ParseJson((JArray)next, expressionTree);
                }
                else if (next.Type == JTokenType.Object)
                {
                    return ParseJson((JObject)next, expressionTree);
                }
                else
                {
                    return next.ToString();
                }
            }
        }
    }
}
