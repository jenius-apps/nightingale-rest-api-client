using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nightingale.Core.Models;
using System;
using System.Threading.Tasks;
using Nightingale.Core.Dialogs;

namespace Nightingale.Core.Helpers
{
    public class EnvironmentContainer : Interfaces.IEnvironmentContainer
    {
        private ObservableCollection<Models.Environment> _environments;
        private readonly IDialogService _dialogService;

        public EnvironmentContainer(IDialogService dialogService)
        {
            _dialogService = dialogService
                ?? throw new ArgumentNullException(nameof(dialogService));
        }

        /// <inheritdoc/>
        public async Task<bool> AddVariablePromptAsync(Parameter parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            (bool success, Parameter p) = TryAddVariable(
                parameter.Key,
                parameter.Value);

            if (!success)
            {
                var confirm = await _dialogService.ConfirmParameterOverwriteAsync(p);

                if (confirm)
                {
                    (success, _) = TryAddVariable(
                        parameter.Key,
                        parameter.Value,
                        overwrite: true);
                }
            }

            if (success)
            {
                parameter.Value = "{{" + parameter.Key + "}}";
            }

            return success;
        }

        /// <inheritdoc/>
        public IList<Parameter> GetActiveVariables()
        {
            var result = new List<Parameter>();

            if (_environments == null || _environments.Count == 0)
            {
                return result;
            }

            Models.Environment baseEnvironment = _environments
                .Where(x => x.EnvironmentType == Models.EnvType.Base)
                .FirstOrDefault();

            ExtractActiveVariables(baseEnvironment, result);

            Models.Environment activeEnvironment = _environments
                .Where(x => x.IsActive && x.EnvironmentType == Models.EnvType.Sub)
                .FirstOrDefault();

            ExtractActiveVariables(activeEnvironment, result);
            
            return result;
        }

        /// <inheritdoc/>
        public (bool, Parameter) TryAddVariable(string key, string value, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return (false, null);
            }

            var activeEnv = _environments.FirstOrDefault(x => x.IsActive);
            if (activeEnv == null)
            {
                activeEnv = _environments.FirstOrDefault(x => x.EnvironmentType == EnvType.Base);
            }

            if (activeEnv == null)
            {
                // it should never come to this,
                // but just in case.
                return (false, null);
            }

            var existing = activeEnv.Variables?.FirstOrDefault(x => key.Equals(x.Key, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                var newVar = new Parameter(true, key, value, ParamType.EnvVariable);
                activeEnv.Variables.Add(newVar);
                return (true, newVar);
            }

            if (overwrite)
            {
                existing.Value = value;
                return (true, existing);
            }

            return (false, existing);
        }

        /// <inheritdoc/>
        public void SetEnvironmentList(ObservableCollection<Models.Environment> environments)
        {
            _environments = environments;
        }

        /// <inheritdoc/>
        public void UpdateVariableValue(string variableName, string newValue, bool shallow = true)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                return;
            }

            // TODO don't do this
            foreach (var environment in _environments)
            {
                foreach (var variable in environment.Variables)
                {
                    if (variable.Key == variableName)
                    {
                        variable.Value = newValue;

                        if (shallow)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void ExtractActiveVariables(Models.Environment environment, IList<Parameter> result)
        {
            if (environment?.Variables == null || environment.Variables.Count == 0)
            {
                return;
            }

            foreach (Parameter variable in environment.Variables)
            {
                if (!variable.Enabled)
                {
                    continue;
                }

                Parameter duplicate = result.Where(r => r.Key == variable.Key).FirstOrDefault();
                if (duplicate == null)
                {
                    result.Add(new Parameter(true, variable.Key, variable.Value, variable.Type));
                }
                else
                {
                    int indexOfDup = result.IndexOf(duplicate);
                    result[indexOfDup].Value = variable.Value;
                }
            }
        }
    }
}
