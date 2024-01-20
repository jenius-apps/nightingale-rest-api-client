using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Nightingale.Modifiers
{
    public class EnvironmentListModifier : IEnvironmentListModifier
    {
        private readonly string _baseEnvName;
        private readonly string _baseEnvIcon = "🌍";
        private readonly string _defaultEnvIcon = "🌐";
        private readonly IEnvironmentStorageAccessor _environmentStorageAccessor;

        public EnvironmentListModifier(IEnvironmentStorageAccessor environmentStorageAccessor)
        {
            _baseEnvName = ResourceLoader.GetForCurrentView().GetString("Base");
            _environmentStorageAccessor = environmentStorageAccessor ?? throw new System.ArgumentNullException(nameof(environmentStorageAccessor));
        }

        public Environment CloneEnvironment(IList<Environment> list, Environment source)
        {
            if (list == null || source == null)
            {
                return null;
            }

            var newEnv = source.DeepClone() as Environment;
            list.Add(newEnv);
            return newEnv;
        }

        public Environment AddNewEnvironment(IList<Environment> list, string newEnvName, string icon, bool isBase = false)
        {
            if (list == null || string.IsNullOrWhiteSpace(newEnvName))
            {
                return null;
            }

            var newEnv = new Environment()
            {
                Name = newEnvName.Trim(),
                Icon = string.IsNullOrWhiteSpace(icon) ? _defaultEnvIcon : icon.Trim(),
                EnvironmentType = isBase ? EnvType.Base : EnvType.Sub,
                IsActive = isBase,
                Status = ModifiedStatus.New
            };

            list.Add(newEnv);
            return newEnv;
        }

        public void AddBaseEnvironment(IList<Environment> list)
        {
            AddNewEnvironment(list, _baseEnvName, _baseEnvIcon, true);
        }

        public async Task DeleteEnvironmentAsync(IList<Environment> list, Environment forDeletion)
        {
            if (list == null || list.Count == 0 || forDeletion == null)
            {
                return;
            }

            list.Remove(forDeletion);

            if (forDeletion.IsActive)
            {
                ResetActive(list);
            }

            await _environmentStorageAccessor.DeleteEnvironmentAsync(forDeletion);
        }

        private void ResetActive(IList<Environment> list)
        {
            if (list == null)
            {
                return;
            }

            bool baseFound = false;
            foreach (var e in list)
            {
                if (e.EnvironmentType == EnvType.Base)
                {
                    e.IsActive = true;
                    baseFound = true;
                }
                else
                {
                    e.IsActive = false;
                }
            }

            if (!baseFound)
            {
                AddNewEnvironment(list, _baseEnvName, _baseEnvIcon, true);
            }
        }
    }
}
