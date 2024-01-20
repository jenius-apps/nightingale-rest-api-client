using Nightingale.Core.Models;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Interfaces
{
    public interface IEnvironment : IStorageItem
    {
        /// <summary>
        /// The type of environment, such as whether
        /// it is a base environment or not.
        /// </summary>
        EnvType EnvironmentType { get; set; }

        /// <summary>
        /// List of environment variables.
        /// </summary>
        ObservableCollection<Parameter> Variables { get; }

        /// <summary>
        /// Boolean for if given environment is active.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Name of environment.
        /// </summary>
        string Name { get; set; }
    }
}
