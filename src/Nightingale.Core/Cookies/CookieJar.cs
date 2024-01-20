using Nightingale.Core.Cookies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Nightingale.Core.Cookies
{
    /// <summary>
    /// Class that holds a list of <see cref="Cookie"/>
    /// objects.
    /// </summary>
    public class CookieJar : ICookieJar
    {
        private ObservableCollection<Cookie> _cookies;

        /// <inheritdoc/>
        public IList<Cookie> GetCookies(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl) 
                || !Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute)
                || _cookies == null)
            {
                return new List<Cookie>();
            }

            var uri = new Uri(baseUrl);
            string host = uri.Host;

            return _cookies.Where(x => host.Contains(x.Domain)).ToList();
        }

        /// <inheritdoc/>
        public void SetCookies(ObservableCollection<Cookie> cookies)
        {
            _cookies = cookies;
        }

        /// <inheritdoc/>
        public ObservableCollection<Cookie> GetCookieList()
        {
            return _cookies;
        }

        /// <inheritdoc/>
        public void AddCookies(IList<Cookie> cookies)
        {
            if (_cookies == null || cookies == null || cookies.Count == 0)
            {
                return;
            }

            foreach (var cookie in cookies)
            {
                if (!_cookies.Any(x => x.Domain == cookie.Domain))
                {
                    _cookies.Add(cookie);
                    continue;
                }

                var cookieWithSameName = _cookies
                    .Where(x => x.Domain == cookie.Domain && x.Name == cookie.Name)
                    .FirstOrDefault();

                if (cookieWithSameName == null)
                {
                    _cookies.Add(cookie);
                    continue;
                }

                cookieWithSameName.Raw = cookie.Raw;
            }
        }

        /// <inheritdoc/>
        public string GetCookieString(string baseUrl)
        {
            var cookieList = GetCookies(baseUrl);
            if (cookieList == null || cookieList.Count == 0)
            {
                return null;
            }

            return string.Join(";", cookieList.Select(cookie => $"{cookie.Name}={cookie.Value}"));
        }
    }
}
