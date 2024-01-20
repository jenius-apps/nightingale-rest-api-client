using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class RequestBodyStorageAccessor : IRequestBodyStorageAccessor
    {
        private readonly IStorage _storage;
        private readonly IParameterStorageAccessor _parameterAccessor;

        public RequestBodyStorageAccessor(
            IStorage storage,
            IParameterStorageAccessor parameterAccessor)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _parameterAccessor = parameterAccessor ?? throw new ArgumentNullException(nameof(parameterAccessor));
        }

        public async Task DeleteBodyAsync(RequestBody body)
        {
            if (body == null)
            {
                return;
            }

            await _storage.DeleteAsync(body);

            foreach (Parameter p in body.FormEncodedData)
            {
                await _parameterAccessor.DeleteParameterAsync(p);
            }
        }

        public async Task DeleteAllAsync(IList<string> parentIds)
        {
            IList<RequestBody> forDeletion = await _storage.GetAsync<RequestBody>(parentIds);
            await _parameterAccessor.DeleteAllAsync(forDeletion.Select(x => x.Id).ToList());
            await _storage.DeleteAllAsync<RequestBody>(parentIds);
        }

        public async Task<RequestBody> GetBodyAsync(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new RequestBody(isNew: true);
            }

            RequestBody result = await _storage.GetAsync<RequestBody>(parentId);

            if (result == null)
            {
                return new RequestBody(isNew: true);
            }

            IList<Parameter> formData = await _parameterAccessor.GetParametersAsync(result.Id);

            foreach (Parameter p in formData)
            {
                if (p.Type == ParamType.FormEncodedData)
                {
                    result.FormEncodedData.Add(p);
                }
            }

            return result;
        }

        public async Task SaveBodyAsync(RequestBody body, string parentId)
        {
            if (body == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            body.ParentId = parentId;
            string bodyId = await _storage.SaveAsync(body);

            foreach (Parameter p in body.FormEncodedData)
            {
                await _parameterAccessor.SaveParameterAsync(p, bodyId);
            }
        }
    }
}
