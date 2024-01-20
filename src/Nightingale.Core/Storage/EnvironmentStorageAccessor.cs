using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class EnvironmentStorageAccessor : IEnvironmentStorageAccessor
    {
        private readonly IStorage _storage;
        private readonly IParameterStorageAccessor _parameterAccessor;

        public EnvironmentStorageAccessor(
            IStorage storage,
            IParameterStorageAccessor parameterAccessor)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _parameterAccessor = parameterAccessor ?? throw new ArgumentNullException(nameof(parameterAccessor));
        }

        public async Task DeleteEnvironmentAsync(Models.Environment environment)
        {
            if (environment == null)
            {
                return;
            }

            foreach (Parameter p in environment.Variables)
            {
                await _parameterAccessor.DeleteParameterAsync(p);
            }

            await _storage.DeleteAsync(environment);
        }

        public async Task DeleteAllAsync(IList<string> parentIds)
        {
            IList<Models.Environment> forDeletion = await _storage.GetAsync<Models.Environment>(parentIds);
            await _parameterAccessor.DeleteAllAsync(forDeletion.Select(x => x.Id).ToList());
            await _storage.DeleteAllAsync<Models.Environment>(parentIds);
        }

        public async Task<IList<Models.Environment>> GetEnvironmentsAsync(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<Models.Environment>();
            }

            IList<Models.Environment> environments = await _storage.GetCollectionAsync<Models.Environment>(parentId);

            foreach (Models.Environment env in environments)
            {
                await LoadParametersAsync(env);
            }

            return environments;
        }

        public async Task SaveEnvironmentAsync(Models.Environment environment, string parentId)
        {
            if (environment == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            environment.ParentId = parentId;
            string envId = await _storage.SaveAsync(environment);

            foreach (Parameter p in environment.Variables)
            {
                await _parameterAccessor.SaveParameterAsync(p, envId);
            }
        }

        private async Task LoadParametersAsync(Models.Environment environment)
        {
            if (environment == null || string.IsNullOrWhiteSpace(environment.Id))
            {
                return;
            }

            IList<Parameter> parameters = await _parameterAccessor.GetParametersAsync(environment.Id);

            foreach (Parameter p in parameters)
            {
                if (p.Type == ParamType.EnvVariable)
                {
                    environment.Variables.Add(p);
                }
            }
        }
    }
}
