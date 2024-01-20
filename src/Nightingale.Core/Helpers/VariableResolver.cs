using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Nightingale.Core.Auth;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;

namespace Nightingale.Core.Helpers
{
    public class VariableResolver : IVariableResolver
    {
        private readonly IEnvironmentContainer _environmentContainer;
        private IList<Parameter> _activeVariables;

        public VariableResolver(IEnvironmentContainer environmentContainer)
        {
            _environmentContainer = environmentContainer ?? throw new ArgumentNullException(nameof(environmentContainer));
        }

        public void UpdateEnvironmentVariablesCache()
        {
            _activeVariables = _environmentContainer.GetActiveVariables();
        }

        public void ResolveAllVariables(Item workspaceRequest)
        {
            if (workspaceRequest == null)
            {
                throw new ArgumentNullException(nameof(workspaceRequest));
            }

            UpdateEnvironmentVariablesCache();

            workspaceRequest.Url.Base = this.ResolveVariable(workspaceRequest.Url.Base, useCache: true);
            this.ResolveAllVariables(workspaceRequest.Body);
            this.ResolveAllVariables(workspaceRequest.Auth);

            //this.ResolveAllVariables(result.ApiTests);
            this.ResolveAllVariables(workspaceRequest.Url.Queries);
            this.ResolveAllVariables(workspaceRequest.Headers);
            this.ResolveAllVariables(workspaceRequest.ChainingRules);
        }

        public string ResolveVariable(string variableName, IList<Parameter> environmentVariables = null, bool useCache = false)
        {
            if (string.IsNullOrWhiteSpace(variableName) || !variableName.Contains("{{") || !variableName.Contains("}}"))
            {
                return variableName;
            }

            IList<Parameter> allActiveVariables;

            if (environmentVariables != null)
            {
                allActiveVariables = environmentVariables;
            }
            else if (useCache)
            {
                if (_activeVariables == null)
                {
                    _activeVariables = _environmentContainer.GetActiveVariables();
                }

                allActiveVariables = _activeVariables;
            }
            else
            {
                allActiveVariables = _environmentContainer.GetActiveVariables();
            }


            return ResolveVariable(variableName, allActiveVariables);
        }

        public static string ResolveVariable(string variableName, IList<Parameter> environmentVariables)
        {
            if (string.IsNullOrWhiteSpace(variableName) 
                || !variableName.Contains("{{") 
                || !variableName.Contains("}}")
                || environmentVariables == null)
            {
                return variableName;
            }

            var result = variableName.ToString();

            foreach (Parameter p in environmentVariables)
            {
                string key = "{{" + p.Key + "}}";
                result = result.Replace(key, p.Value);
            }

            return result;
        }

        private void ResolveAllVariables(ObservableCollection<Parameter> parameterList)
        {
            if (parameterList == null || parameterList.Count == 0)
            {
                return;
            }

            Parallel.ForEach(parameterList, ResolveParameterVariables);
        }

        private void ResolveParameterVariables(Parameter parameter)
        {
            if (parameter == null)
            {
                return;
            }

            parameter.Key = this.ResolveVariable(parameter.Key, useCache: true);
            parameter.Value = this.ResolveVariable(parameter.Value, useCache: true);
        }

        private void Resolve(Authentication a)
        {
            if (a == null)
            {
                return;
            }

            foreach (var key in AuthConstants.Props)
            {
                var resolvedValue = ResolveVariable(a.GetProp(key));
                a.SetProp(key, resolvedValue);
            }
        }

        private void ResolveAllVariables(RequestBody requestBody)
        {
            if (requestBody == null)
            {
                return;
            }

            this.ResolveAllVariables(requestBody.FormEncodedData);
            requestBody.JsonBody = this.ResolveVariable(requestBody.JsonBody, useCache: true);
            requestBody.XmlBody = this.ResolveVariable(requestBody.XmlBody, useCache: true);
            requestBody.TextBody = this.ResolveVariable(requestBody.TextBody, useCache: true);
        }

        /// <inheritdoc/>
        public Authentication ResolveAllVariables(Authentication auth)
        {
            if (auth == null)
            {
                return null;
            }

            var other = auth.DeepClone() as Authentication;
            if (other == null)
            {
                return null;
            }

            UpdateEnvironmentVariablesCache();
            Resolve(other);
            return other;
        }
    }
}
