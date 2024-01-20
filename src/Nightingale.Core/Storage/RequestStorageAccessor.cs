using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;

namespace Nightingale.Core.Storage
{
    public class RequestStorageAccessor : IRequestStorageAccessor
    {
        private readonly IStorage _storage;
        private readonly IParameterStorageAccessor _parameterAccessor;
        private readonly IApiTestStorageAccessor _testAccessor;
        private readonly IAuthStorageAccessor _authAccessor;
        private readonly IRequestBodyStorageAccessor _bodyAccessor;

        public RequestStorageAccessor(
            IStorage storage,
            IParameterStorageAccessor parameterAccessor,
            IApiTestStorageAccessor testAccessor,
            IAuthStorageAccessor authAccessor,
            IRequestBodyStorageAccessor requestBodyStorageAccessor)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _parameterAccessor = parameterAccessor ?? throw new ArgumentNullException(nameof(parameterAccessor));
            _testAccessor = testAccessor ?? throw new ArgumentNullException(nameof(testAccessor));
            _authAccessor = authAccessor ?? throw new ArgumentNullException(nameof(authAccessor));
            _bodyAccessor = requestBodyStorageAccessor ?? throw new ArgumentNullException(nameof(requestBodyStorageAccessor));
        }

        public async Task Hydrate<T>(T request) where T : IWorkspaceRequest
        {
            if (request == null || request.IsHydrated || string.IsNullOrWhiteSpace(request.Id))
            {
                return;
            }

            await LoadParametersAsync(request);
            await LoadApiTestsAsync(request);
            request.Authentication = await _authAccessor.GetAuthenticationAsync(request.Id);
            request.Body = await _bodyAccessor.GetBodyAsync(request.Id);
            request.IsHydrated = true;
        }

        public async Task<IList<T>> GetRequestsAsync<T>(string parentId) where T : IWorkspaceRequest
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<T>();
            }

            IList<T> requests = await _storage.GetCollectionAsync<T>(parentId);

            foreach (T request in requests)
            {
                // migrate to new method
                if (string.IsNullOrWhiteSpace(request.Method))
                {
                    try
                    {
                        request.Method = Http.Method.Defaults[request.MethodIndex];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        request.Method = "GET";
                    }
                }

                await LoadParametersAsync(request);
                await LoadApiTestsAsync(request);
                request.Authentication = await _authAccessor.GetAuthenticationAsync(request.Id);
                request.Body = await _bodyAccessor.GetBodyAsync(request.Id);

                // Migrate to new url
                if (request.Url.Base == null && request.BaseUrl != null)
                {
                    request.Url.Base = request.BaseUrl;
                    request.BaseUrl = null;

                    if (request.Url.Queries.Count > 0 && !request.Url.Base.EndsWith("?"))
                    {
                        request.Url.Base += "?";
                    }
                }
            }

            return requests;
        }

        public async Task SaveRequestAsync<T>(T request, string parentId) where T : IWorkspaceRequest
        {
            if (request == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            request.ParentId = parentId;

            string requestId = await _storage.SaveAsync(request);

            List<Parameter> allParameters = new List<Parameter>();
            allParameters.AddRange(request.Headers);
            allParameters.AddRange(request.ChainingRules);

            foreach (Parameter p in allParameters)
            {
                await _parameterAccessor.SaveParameterAsync(p, requestId);
            }

            foreach (ApiTest t in request.ApiTests)
            {
                await _testAccessor.SaveTestAsync(t, requestId);
            }

            await _authAccessor.SaveAuthenticationAsync(request.Authentication, requestId);
            await _bodyAccessor.SaveBodyAsync(request.Body, requestId);
        }

        public async Task DeleteRequestAsync<T>(T request) where T : IWorkspaceRequest
        {
            if (request == null)
            {
                return;
            }

            await _storage.DeleteAsync(request);

            var allParameters = new List<Parameter>();
            allParameters.AddRange(request.Headers);
            allParameters.AddRange(request.ChainingRules);

            foreach (Parameter p in allParameters)
            {
                await _parameterAccessor.DeleteParameterAsync(p);
            }

            foreach (ApiTest t in request.ApiTests)
            {
                await _testAccessor.DeleteTestAsync(t);
            }

            await _authAccessor.DeleteAuthenticationAsync(request.Authentication);
            await _bodyAccessor.DeleteBodyAsync(request.Body);
        }

        public async Task DeleteAllAsync<T>(IList<string> parentIds) where T : IWorkspaceRequest
        {
            IList<T> forDeletion = await _storage.GetAsync<T>(parentIds);
            IList<string> forDeletionIds = forDeletion.Select(x => x.Id).ToList();

            // delete all properties
            await _parameterAccessor.DeleteAllAsync(forDeletionIds);
            await _testAccessor.DeleteAllAsync(forDeletionIds);
            await _authAccessor.DeleteAllAsync(forDeletionIds);
            await _bodyAccessor.DeleteAllAsync(forDeletionIds);

            await _storage.DeleteAllAsync<T>(parentIds);
        }


        private async Task LoadParametersAsync<T>(T request) where T : IWorkspaceRequest
        {
            if (request == null)
            {
                return;
            }

            IList<Parameter> allParams = await _parameterAccessor.GetParametersAsync(request.Id);
            
            foreach (Parameter p in allParams)
            {
                switch (p.Type)
                {
                    case ParamType.Header:
                        request.Headers.Add(p);
                        break;
                    case ParamType.Parameter:
                        // migrate to new url
                        request.Url.Queries.Add(p);
                        await _parameterAccessor.DeleteParameterAsync(p);
                        break;
                    case ParamType.ChainingRule:
                        request.ChainingRules.Add(p);
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task LoadApiTestsAsync<T>(T request) where T : IWorkspaceRequest
        {
            if (request == null)
            {
                return;
            }

            IList<ApiTest> tests = await _testAccessor.GetApiTestsAsync(request.Id);

            foreach (ApiTest test in tests)
            {
                request.ApiTests.Add(test);
            }
        }
    }
}
