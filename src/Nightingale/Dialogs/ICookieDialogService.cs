using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Dialogs
{
    /// <summary>
    /// An interface for a service
    /// that controls the cookie dialog.
    /// </summary>
    public interface ICookieDialogService
    {
        /// <summary>
        /// Opens the cookie dialog.
        /// </summary>
        /// <returns>A task representing asynchronous work.</returns>
        Task OpenCookieDialog();
    }
}
