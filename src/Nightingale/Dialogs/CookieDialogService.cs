using Nightingale.Core.Cookies;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Dialogs
{
    /// <summary>
    /// A class that implements a
    /// cookie dialog service.
    /// </summary>
    public class CookieDialogService : BaseDialogService, ICookieDialogService
    {
        private readonly ICookieJar _cookieJar;

        /// <summary>
        /// Initializes an instance of <see cref="CookieDialogService"/>.
        /// </summary>
        /// <param name="cookieJar"></param>
        public CookieDialogService(ICookieJar cookieJar)
        {
            _cookieJar = cookieJar ?? throw new ArgumentNullException(nameof(cookieJar));
        }

        /// <inheritdoc/>
        public async Task OpenCookieDialog()
        {
            var list = _cookieJar.GetCookieList();

            if (IsDialogActive || list == null)
            {
                return;
            }

            IsDialogActive = true;
            var dialog = new CookiesDialog();
            dialog.ViewModel = new ViewModels.CookiesViewModel();
            dialog.ViewModel.Cookies = list;
            await dialog.ShowAsync();
            IsDialogActive = false;
        }
    }
}
