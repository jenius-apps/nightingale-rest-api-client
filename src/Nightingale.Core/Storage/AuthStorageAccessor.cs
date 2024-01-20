using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nightingale.Core.Auth;

namespace Nightingale.Core.Storage
{
    public class AuthStorageAccessor : IAuthStorageAccessor
    {
        private readonly IStorage _storage;

        public AuthStorageAccessor(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task DeleteAuthenticationAsync(Authentication auth)
        {
            if (auth == null)
            {
                return;
            }

            await _storage.DeleteAsync(auth);
        }

        public async Task<Authentication> GetAuthenticationAsync(string parentId, bool createNew = true)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new Authentication();
            }

            Authentication result = await _storage.GetAsync<Authentication>(parentId, passReference: true);
            if (result == null)
            {
                return new Authentication();
            }

            // Migrate to new schema
            result.SetProp(AuthConstants.BasicUsername, result.BasicUsername);
            result.SetProp(AuthConstants.BasicPassword, result.BasicPassword);

            result.SetProp(AuthConstants.OAuth1AccessToken, result.OAuth1AccessToken);
            result.SetProp(AuthConstants.OAuth1ConsumerKey, result.OAuth1ConsumerKey);
            result.SetProp(AuthConstants.OAuth1ConsumerSecret, result.OAuth1ConsumerSecret);
            result.SetProp(AuthConstants.OAuth1TokenSecret, result.OAuth1TokenSecret);

            result.SetProp(AuthConstants.OAuth2AccessToken, result.OAuth2AccessToken);
            result.SetProp(AuthConstants.OAuth2AccessTokenUrl, result.OAuth2AccessTokenUrl);
            result.SetProp(AuthConstants.OAuth2AuthUrl, result.OAuth2AuthUrl);
            result.SetProp(AuthConstants.OAuth2CallbackUrl, result.OAuth2CallbackUrl);
            result.SetProp(AuthConstants.OAuth2ClientId, result.OAuth2ClientId);
            result.SetProp(AuthConstants.OAuth2ClientSecret, result.OAuth2ClientSecret);
            result.SetProp(AuthConstants.OAuth2GrantType, result.OAuth2GrantType.ToString());
            result.SetProp(AuthConstants.OAuth2Scope, result.OAuth2Scope);

            result.SetProp(AuthConstants.BearerToken, result.BearerToken);

            result.SetProp(AuthConstants.DigestUsername, result.DigestUsername);
            result.SetProp(AuthConstants.DigestPassword, result.DigestPassword);

            // Part of the migration.
            // Deletes data from old schema
            await _storage.DeleteAsync(result);
            
            return result ?? (createNew ? new Authentication() : null);
        }

        public async Task DeleteAllAsync(IList<string> parentIds)
        {
            await _storage.DeleteAllAsync<Authentication>(parentIds);
        }

        public async Task SaveAuthenticationAsync(Authentication auth, string parentId)
        {
            if (auth == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            auth.ParentId = parentId;
            await _storage.SaveAsync(auth);
        }
    }
}
