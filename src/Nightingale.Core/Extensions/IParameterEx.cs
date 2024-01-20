using Nightingale.Core.Helpers;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nightingale.Core.Extensions
{
    public static class IParameterEx
    {
        public static void AddDisabled(this IList<Parameter> list, ParamType type)
        {
            list.Add(new Parameter(isEnabled: false, type: type));
        }

        public static void PopulateIfEmpty(this IList<Parameter> list, ParamType type = ParamType.Parameter)
        {
            if (list.Count == 0)
            {
                list.AddDisabled(type);
            }
        }

        public static string GetDisplayCount(this IList<Parameter> list)
        {
            int count = list.Count(x => !string.IsNullOrWhiteSpace(x.Key) || !string.IsNullOrWhiteSpace(x.Value));
            return count == 0 ? "" : count.ToString();
        }

        public static IEnumerable<Parameter> GetActive(this IList<Parameter> parameters)
        {
            return parameters
                .Where(x => x.Enabled)
                .Where(x => !string.IsNullOrWhiteSpace(x.Key));
        }

        public static string ToQueryString(this IList<Parameter> parameters, bool escapeUri = true)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var param in parameters.GetActive())
            {
                sb.Append(sb.Length == 0 ? "?" : "&");

                string key = param.Key;
                string value = param.Value;

                if (escapeUri)
                {
                    key = Uri.EscapeUriString(key);
                    value = Uri.EscapeUriString(value);
                }

                sb.AppendFormat("{0}={1}", key, value);
            }

            return sb.ToString();
        }

        public static void AddContentTypeHeader(this IList<Parameter> list, RequestBodyType bodyType)
        {
            string headerValue = "";
            switch (bodyType)
            {
                case RequestBodyType.Json:
                    headerValue = MimeConstant.Json; break;
                case RequestBodyType.Xml:
                    headerValue = MimeConstant.Xml; break;
                case RequestBodyType.FormEncoded:
                    headerValue = MimeConstant.FormEncoded; break;
                case RequestBodyType.Binary:
                    headerValue = MimeConstant.OctetStream; break;
                case RequestBodyType.FormData:
                    headerValue = MimeConstant.FormData; break;
                case RequestBodyType.Text:
                    headerValue = MimeConstant.TextPlain; break;
            }

            IList<Parameter> contentHeaders = list
                .Where(x => x.Key.ToLower() == "content-type")?
                .ToList() ?? new List<Parameter>();

            if (contentHeaders.Count > 0)
            {
                // If the content type was customized 
                // by the user, don't touch it.
                if (!MimeConstant.IsStandardMime(contentHeaders[0].Value))
                {
                    return;
                }

                list.Remove(contentHeaders[0]);
                list.PopulateIfEmpty(contentHeaders[0].Type);
            }

            if (bodyType == RequestBodyType.None)
            {
                return;
            }

            list.Insert(0, new Parameter(true, "Content-Type", headerValue, ParamType.Header));
        }
    }
}
