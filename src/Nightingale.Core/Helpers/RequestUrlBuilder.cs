using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;

namespace Nightingale.Core.Helpers
{
    public class RequestUrlBuilder : IRequestUrlBuilder
    {
        private readonly IVariableResolver _variableResolver;

        public RequestUrlBuilder(IVariableResolver variableResolver)
        {
            _variableResolver = variableResolver ?? throw new ArgumentNullException(nameof(variableResolver));
        }

        public string GetPreviewUrl(Item request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url?.Base))
            {
                return "";
            }

            var result = _variableResolver.ResolveVariable(request.Url.ToString());

            try
            {
                result = Uri.EscapeUriString(result);
            }
            catch (UriFormatException)
            {
                // catch issues such as string being too long.
            }

            return result;
        }
    }
}
