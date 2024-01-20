using Newtonsoft.Json;
using Nightingale.Core.Auth;
using Nightingale.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Nightingale.Core.Models
{
    public enum AuthType
    {
        None,
        Basic,
        OAuth1,
        OAuth2,
        Bearer,
        Digest,
        InheritParent
    }

    public enum GrantType
    {
        client_credentials,
        authorization_code,
        implicit_flow
    }

    public class Authentication : ModifiableBase, IStorageItem, IDeepCloneable, IAuth
    {
        public event EventHandler TypeChanged;

        public string Id { get; set; }
        public string ParentId { get; set; }
        public AuthType AuthType
        {
            get => _type;
            set
            {
                _type = value;
                TypeChanged?.Invoke(this, new EventArgs());
            }
        }
        private AuthType _type;

        // Basic
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BasicUsername { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BasicPassword { get; set; }

        // OAuth1
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth1ConsumerKey { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth1ConsumerSecret { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth1AccessToken { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth1TokenSecret { get; set; }

        // OAuth2
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2AccessTokenUrl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2AuthUrl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2ClientId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2ClientSecret { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2Scope { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2AccessToken { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OAuth2CallbackUrl { get; set; }
        public GrantType OAuth2GrantType { get; set; }

        // Bearer
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BearerToken { get; set; }

        // Digest
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DigestUsername { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DigestPassword { get; set; }

        public Dictionary<string, string> AuthProperties { get; set; }

        public object DeepClone()
        {
            var result = new Authentication
            {
                AuthType = this.AuthType,
                AuthProperties = this.AuthProperties != null 
                    ? new Dictionary<string, string>(this.AuthProperties) 
                    : new Dictionary<string, string>(),
            };

            return result;
        }
    }
}
