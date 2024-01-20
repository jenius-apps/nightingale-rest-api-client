using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class ParameterStorageAccessor : IParameterStorageAccessor
    {
        private readonly IStorage _storage;

        public ParameterStorageAccessor(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task DeleteParameterAsync(Parameter parameter)
        {
            if (parameter == null)
            {
                return;
            }

            await _storage.DeleteAsync(parameter);
        }

        public async Task DeleteAllAsync(IList<string> parentIds)
        {
            await _storage.DeleteAllAsync<Parameter>(parentIds);
        }

        public async Task<IList<Parameter>> GetParametersAsync(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<Parameter>();
            }

            return await _storage.GetCollectionAsync<Parameter>(parentId);
        }

        public async Task SaveParameterAsync(Parameter parameter, string parentId)
        {
            if (parameter == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(parameter.Key) && string.IsNullOrWhiteSpace(parameter.Value))
            {
                return;
            }

            parameter.ParentId = parentId;
            await _storage.SaveAsync(parameter);
        }
    }
}
