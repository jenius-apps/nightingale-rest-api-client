using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Dialogs
{
    /// <summary>
    /// An abstract class that provides
    /// the basis of dialog service classes.
    /// </summary>
    public abstract class BaseDialogService
    {
        /// <summary>
        /// Gets or sets the flag for when a dialog
        /// is active or not.
        /// </summary>
        public static bool IsDialogActive { get; set; }
    }
}
