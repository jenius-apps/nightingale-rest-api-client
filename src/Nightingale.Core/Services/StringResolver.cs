using Nightingale.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Core.Services
{
    public class StringResolver : IStringResolver
    {
        public string ResolveString(string raw, IList<IParameter> variables)
        {
            if (string.IsNullOrWhiteSpace(raw) || variables == null || variables.Count == 0)
            {
                return raw;
            }

            var result = raw.ToString();

            foreach (IParameter v in variables)
            {
                if (v.Enabled)
                {
                    string key = "{{" + v.Key + "}}";
                    result = result.Replace(key, v.Value);
                }
            }

            return result;
        }

        public Task<string> ResolveStringAsync(string raw, IList<IParameter> variables)
        {
            Task<string> t = Task.Run(() =>
            {
                return ResolveString(raw, variables);
            });

            return t;
        }
    }
}
