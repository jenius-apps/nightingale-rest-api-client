using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Extensions
{
    public static class EnvironmentListEx
    {
        public static Environment GetActive(this IList<Environment> list)
        {
            foreach (Environment env in list)
            {
                if (env.IsActive)
                {
                    return env;
                }
            }

            return list.Where(x => x.EnvironmentType == EnvType.Base).SingleOrDefault();
        }

        public static void SetActive(this IList<Environment> list, Environment selectedEnv)
        {
            if (selectedEnv == null || !list.Contains(selectedEnv))
            {
                list.ResetActiveToBase();
                return;
            }

            foreach (Environment env in list)
            {
                env.IsActive = selectedEnv == env;
            }
        }

        public static void ResetActiveToBase(this IList<Environment> list)
        {
            Environment baseEnv = list.Where(x => x.EnvironmentType == EnvType.Base).FirstOrDefault();

            if (baseEnv == null)
            {
                return;
            }

            list.SetActive(baseEnv);
        }
    }
}
