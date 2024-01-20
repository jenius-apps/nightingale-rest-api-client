using Nightingale.Core.Cookies.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Nightingale.Core.Cookies
{
    /// <summary>
    /// An interface defining an object
    /// that contains cookies.
    /// </summary>
    public interface ICookieJar
    {
        /// <summary>
        /// Sets the list of cookies in the cookie jar.
        /// </summary>
        /// <param name="cookies">A list of cookies.</param>
        void SetCookies(ObservableCollection<Cookie> cookies);

        /// <summary>
        /// Returns cookies associated with
        /// the base URL's domain.
        /// </summary>
        /// <param name="baseUrl">A base URL.</param>
        /// <returns>List of cookies associated with base URL.</returns>
        IList<Cookie> GetCookies(string baseUrl);

        /// <summary>
        /// Returns the cookies for the 
        /// given URL in string format. E.g.
        /// cookie1=value1; cookie2=value2
        /// </summary>
        /// <param name="baseUrl">A base URL associated with cookies.</param>
        /// <returns>Cookies in string format.</returns>
        string GetCookieString(string baseUrl);

        /// <summary>
        /// Returns observable collection
        /// of the cookie list.
        /// </summary>
        /// <returns>List of cookies.</returns>
        ObservableCollection<Cookie> GetCookieList();

        /// <summary>
        /// Adds cookies into jar if they are unique
        /// in domain and cookie name. If a cookie already
        /// exists, then the cookie is updated.
        /// </summary>
        /// <param name="cookies">List of cookies to add.</param>
        void AddCookies(IList<Cookie> cookies);
    }
}
