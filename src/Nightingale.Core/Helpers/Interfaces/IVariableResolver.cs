using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IVariableResolver
    {
        string ResolveVariable(
            string variableName,
            IList<Parameter> environmentVariables = null,
            bool useCache = false);

        void ResolveAllVariables(Item workspaceRequest);

        /// <summary>
        /// Returns a copy of the authentication
        /// with all variables resolved.
        /// </summary>
        Authentication ResolveAllVariables(Authentication auth);

        void UpdateEnvironmentVariablesCache();
    }
}
