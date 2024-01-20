using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Extensions;
using System;
using System.Net.Http;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;

namespace Nightingale.Core.Client.Builders
{
    /// <summary>
    /// Class for configuring the headers of
    /// a request message.
    /// </summary>
    public class HeaderBuilder : IHeaderBuilder
    {
        private readonly IVariableResolver _varRes;

        public HeaderBuilder(
            IVariableResolver variableResolver)
        {
            _varRes = variableResolver
                ?? throw new ArgumentNullException(nameof(variableResolver));
        }

        /// <inheritdoc/>
        public void SetHeaders(Item request, HttpRequestMessage message, ILogger logger = null)
        {
            var activeHeaders = request.GetHeaders(activeOnly: true);
            foreach (var h in activeHeaders)
            {
                string key = _varRes.ResolveVariable(h.Key, null, useCache: true).Trim();
                string value = _varRes.ResolveVariable(h.Value, null, useCache: true).Trim();
                logger?.Log(key + ": " + value);

                if (key.ToLower() == "authorization")
                {

                    if (request.Auth.AuthType == AuthType.None)
                    {
                        // only use custom auth header if the item's auth setting is set to None
                        message.Headers.TryAddWithoutValidation(key, value);
                    }

                    continue;
                }

                if (key.ToLower() == "user-agent")
                {
                    message.Headers.UserAgent.TryParseAdd(value);
                    continue;
                }

                if (!key.StartsWith("content", StringComparison.OrdinalIgnoreCase))
                {
                    message.Headers.Add(key, value);
                }
            }

        }
    }
}
