using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Auth;
using System;
using System.Web;
using Nightingale.Core.Helpers;
using System.Collections.Generic;

namespace Nightingale.ViewModels
{
    public class AuthControlViewModel : ObservableBase, ILoggable
    {
        private readonly ILogger _logger;
        private readonly Core.Interfaces.IOAuth2TokenRetriever _oauth2TokenRetriever;
        private readonly Core.Auth.IOAuth2TokenRetriever _auth2TokenRetriever;
        private readonly IOAuth1TokenRetriever _oauth1TokenRetriever;
        
        private Authentication _activeAuthModel;
        private string _log;

        public AuthControlViewModel(
            Core.Interfaces.IOAuth2TokenRetriever oauth2TokenRetriever,
            IOAuth1TokenRetriever oauth1TokenRetriever,
            Core.Auth.IOAuth2TokenRetriever auth2TokenRetriever,
            ILogger logger)
        {
            _oauth2TokenRetriever = oauth2TokenRetriever;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oauth1TokenRetriever = oauth1TokenRetriever
                ?? throw new ArgumentNullException(nameof(oauth1TokenRetriever));
            _auth2TokenRetriever = auth2TokenRetriever
                ?? throw new ArgumentNullException(nameof(auth2TokenRetriever));
        }

        public int AuthTypeIndex
        {
            get => ActiveAuthModel == null ? 0 : (int)ActiveAuthModel.AuthType;
            set
            {
                if (ActiveAuthModel == null)
                {
                    return;
                }
                
                ActiveAuthModel.AuthType = value >= 0 && value < Enum.GetNames(typeof(AuthType)).Length ? (AuthType)value : 0;
                RaisePropertyChanged(nameof(IsBasicAuth));
                RaisePropertyChanged(nameof(IsOauth1));
                RaisePropertyChanged(nameof(IsOauth2));
                RaisePropertyChanged(nameof(IsBearerAuth));
                RaisePropertyChanged(nameof(IsDigestAuth));
                RaisePropertyChanged(nameof(IsNoAuth));
                RaisePropertyChanged(nameof(IsParentAuth));
                RaisePropertyChanged(nameof(AuthTypeIndex));
                RaisePropertyChanged(nameof(IsClientCredential));
                RaisePropertyChanged(nameof(IsAuthorizationCode));
                RaisePropertyChanged(nameof(IsImplicitFlow));
            }
        }

        public string BasicUsername
        {
            get => ActiveAuthModel.GetProp(AuthConstants.BasicUsername);
            set => ActiveAuthModel.SetProp(AuthConstants.BasicUsername, value);
        }

        public string BasicPassword
        {
            get => ActiveAuthModel.GetProp(AuthConstants.BasicPassword);
            set => ActiveAuthModel.SetProp(AuthConstants.BasicPassword, value);
        }

        public string OAuth1ConsumerKey
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1ConsumerKey);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth1ConsumerKey, value);
        }

        public string OAuth1ConsumerSecret
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1ConsumerSecret);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth1ConsumerSecret, value);
        }

        public string OAuth1CallbackUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1CallbackUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth1CallbackUrl, value);
        }

        public string OAuth1AuthorizationUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1AuthorizationUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth1AuthorizationUrl, value);
        }

        public string OAuth1AccessTokenUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1AccessTokenUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth1AccessTokenUrl, value);
        }

        public string OAuth1RequestTokenUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1RequestTokenUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth1RequestTokenUrl, value);
        }

        public string OAuth1AccessToken
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1AccessToken);
            set
            {
                ActiveAuthModel.SetProp(AuthConstants.OAuth1AccessToken, value);
                RaisePropertyChanged();
            }
        }

        public string OAuth1TokenSecret
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth1TokenSecret);
            set
            {
                ActiveAuthModel.SetProp(AuthConstants.OAuth1TokenSecret, value);
                RaisePropertyChanged();
            }
        }

        public int OAuth2GrantTypeIndex
        {
            get
            {
                if (ActiveAuthModel == null)
                {
                    return 0;
                }

                return (int)ActiveAuthModel.GetEnumProp<GrantType>(AuthConstants.OAuth2GrantType);
            }
            set
            {
                if (value == -1 || OAuth2GrantTypeIndex == value || ActiveAuthModel == null)
                {
                    return;
                }

                GrantType grantType = value >= 0 && value < Enum.GetNames(typeof(GrantType)).Length ? (GrantType)value : 0;
                ActiveAuthModel.SetProp(AuthConstants.OAuth2GrantType, grantType.ToString());
                RaisePropertyChanged(nameof(OAuth2GrantTypeIndex));
                RaisePropertyChanged(nameof(IsClientCredential));
                RaisePropertyChanged(nameof(IsAuthorizationCode));
                RaisePropertyChanged(nameof(IsImplicitFlow));
            }
        }

        public string OAuth2AccessTokenUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2AccessTokenUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2AccessTokenUrl, value);
        }

        public string OAuth2ClientId
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2ClientId);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2ClientId, value);
        }

        public string OAuth2ClientSecret
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2ClientSecret);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2ClientSecret, value);
        }

        public string OAuth2Scope
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2Scope);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2Scope, value);
        }

        public string OAuth2State
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2State);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2State, value);
        }

        public string OAuth2RefreshToken
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2RefreshToken);
            set
            {
                ActiveAuthModel.SetProp(AuthConstants.OAuth2RefreshToken, value);
                RaisePropertyChanged();
            }
        }

        public string OAuth2CallbackUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2CallbackUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2CallbackUrl, value);
        }

        public string OAuth2AuthUrl
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2AuthUrl);
            set => ActiveAuthModel.SetProp(AuthConstants.OAuth2AuthUrl, value);
        }

        public string OAuth2AccessToken
        {
            get => ActiveAuthModel.GetProp(AuthConstants.OAuth2AccessToken);
            set 
            {
                ActiveAuthModel.SetProp(AuthConstants.OAuth2AccessToken, value);
                RaisePropertyChanged();
            } 
        }

        public string BearerToken
        {
            get => ActiveAuthModel.GetProp(AuthConstants.BearerToken);
            set => ActiveAuthModel.SetProp(AuthConstants.BearerToken, value);
        }

        public string DigestUsername
        {
            get => ActiveAuthModel.GetProp(AuthConstants.DigestUsername);
            set => ActiveAuthModel.SetProp(AuthConstants.DigestUsername, value);
        }

        public string DigestPassword
        {
            get => ActiveAuthModel.GetProp(AuthConstants.DigestPassword);
            set => ActiveAuthModel.SetProp(AuthConstants.DigestPassword, value);
        }

        public Authentication ActiveAuthModel
        {
            get => _activeAuthModel;
            set
            {
                _activeAuthModel = value;
                RaisePropertyChanged(string.Empty);
            }
        }

        public GrantType AuthGrantType
            => ActiveAuthModel?.GetEnumProp<GrantType>(AuthConstants.OAuth2GrantType) ?? GrantType.client_credentials;

        public bool IsNoAuth
        {
            get => ActiveAuthModel?.AuthType == AuthType.None;
        }
            
        public bool IsBasicAuth
        {
            get => ActiveAuthModel?.AuthType == AuthType.Basic;
        }
        public bool IsOauth1
        {
            get => ActiveAuthModel?.AuthType == AuthType.OAuth1;
        }
        public bool IsOauth2
        {
            get => ActiveAuthModel?.AuthType == AuthType.OAuth2;
        }
        public bool IsBearerAuth
        {
            get => ActiveAuthModel?.AuthType == AuthType.Bearer;
        }
        public bool IsDigestAuth
        {
            get => ActiveAuthModel?.AuthType == AuthType.Digest;
        }
        public bool IsParentAuth
        {
            get => ActiveAuthModel?.AuthType == AuthType.InheritParent;
        }
        public bool IsClientCredential
        {
            get => AuthGrantType == GrantType.client_credentials;
        }
        public bool IsImplicitFlow
        {
            get => AuthGrantType == GrantType.implicit_flow;
        }
        public bool IsAuthorizationCode
        {
            get => AuthGrantType == GrantType.authorization_code;
        }

        public string Log
        {
            get => _log;
            set
            {
                _log = value;
                RaisePropertyChanged("Log");
            }
        }

        public async void FetchOauth1Token()
        {
            if (ActiveAuthModel == null)
            {
                return;
            }

            Log = "";
            var responseString = await _oauth1TokenRetriever.GetAccessTokenResultAsync(
                this.OAuth1ConsumerKey,
                this.OAuth1ConsumerSecret,
                this.OAuth1CallbackUrl,
                this.OAuth1RequestTokenUrl,
                this.OAuth1AuthorizationUrl,
                this.OAuth1AccessTokenUrl,
                new OnDemandLogger(this));

            if (string.IsNullOrWhiteSpace(responseString))
            {
                return;
            }

            var queryString = HttpUtility.ParseQueryString(responseString);
            this.OAuth1AccessToken = queryString["oauth_token"];
            this.OAuth1TokenSecret = queryString["oauth_token_secret"];
        }

        public async void FetchToken()
        {
            Log = string.Empty;

            if (ActiveAuthModel is null)
            {
                return;
            }

            try
            {
                // TODO add string property to display log
                Token token = await _oauth2TokenRetriever.GetOAuth2Token(ActiveAuthModel, _logger, force: true);
                OAuth2AccessToken = token.AccessToken;
                OAuth2RefreshToken = token.RefreshToken;
            }
            catch (Exception e)
            {
                OAuth2AccessToken = string.Empty;
                Log = e.Message;
            }
        }

        public async void RefreshToken()
        {
            Log = "";

            if (ActiveAuthModel is null)
            {
                return;
            }

            var otherParams = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(OAuth2ClientId))
            {
                otherParams.Add("client_id", OAuth2ClientId);
            }
            if (!string.IsNullOrWhiteSpace(OAuth2ClientSecret))
            {
                otherParams.Add("client_secret", OAuth2ClientSecret);
            }
            if (!string.IsNullOrWhiteSpace(OAuth2CallbackUrl))
            {
                otherParams.Add("redirect_uri", OAuth2CallbackUrl);
            }

            try
            {
                Token token = await _auth2TokenRetriever.RefreshAccessToken(
                    OAuth2AccessTokenUrl,
                    OAuth2RefreshToken,
                    // MS Graph refresh does not work if any scope if provided. Need more testing to determine how to handle scope.
                    null, 
                    otherParams);

                OAuth2AccessToken = token.AccessToken;
                OAuth2RefreshToken = token.RefreshToken;
            }
            catch (Exception e)
            {
                OAuth2AccessToken = "";
                Log = e.Message;
            }
        }
    }
}
