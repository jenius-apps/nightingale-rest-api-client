using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IEnvironmentContainer
    {
        /// <summary>
        /// Updates occurrences of the variable with the new value.
        /// </summary>
        /// <param name="variableName">The name of the variable to update.</param>
        /// <param name="newValue">The new value to be assigned.</param>
        /// <param name="shallow">Optional. If true, only the first occurrence of the variable name will be updated.
        /// Otherwise, all occurrences will be udpated.</param>
        void UpdateVariableValue(string variableName, string newValue, bool shallow = true);

        /// <summary>
        /// Returns list of active and base environment variables.
        /// Variables with duplicate keys will be overwritten
        /// with workspace-level variables taking the lowest precedent.
        /// Within a given environment list, if there is is a duplicate
        /// key between the active and the base environment, the 
        /// variable in active takes precedent.
        /// </summary>
        /// <returns>List of active environment variables.</returns>
        IList<Parameter> GetActiveVariables();

        void SetEnvironmentList(ObservableCollection<Models.Environment> environments);

        /// <summary>
        /// Adds a new variable into the active environment. Returns true 
        /// and the created variable if successful. If not, returns false
        /// and the existing variable (if it exists).
        /// </summary>
        /// <param name="key">Key of variable.</param>
        /// <param name="value">Value of variable.</param>
        /// <param name="overwrite">
        /// Optional. If true, overwrites variable with idential key name.
        /// If false, no changes are made. Default is false.
        /// </param>
        (bool, Parameter) TryAddVariable(string key, string value, bool overwrite = false);

        /// <summary>
        /// Attempts to add the given parameter
        /// to the active environment. Pops a dialog
        /// if collision is found.
        /// </summary>
        /// <param name="p">The parameter to add.</param>
        /// <returns>True if successful.</returns>
        Task<bool> AddVariablePromptAsync(Parameter p);
    }
}
