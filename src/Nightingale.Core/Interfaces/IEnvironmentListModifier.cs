using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    public interface IEnvironmentListModifier
    {
        /// <summary>
        /// Clones the given source and adds it into the 
        /// environment list.
        /// </summary>
        Environment CloneEnvironment(IList<Environment> list, Environment source);

        Environment AddNewEnvironment(IList<Environment> list, string newEnvName, string icon, bool isBase = false);

        void AddBaseEnvironment(IList<Environment> list);

        Task DeleteEnvironmentAsync(IList<Environment> list, Environment forDeletion);
    }
}
