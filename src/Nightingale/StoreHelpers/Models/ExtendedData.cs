using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.StoreHelpers.Models
{
    /// <summary>
    /// Partial implementation of the real class.
    /// Source: https://docs.microsoft.com/en-us/windows/uwp/monetize/data-schemas-for-store-products
    /// </summary>
    public class ExtendedData
    {
        public SkuAvailability[] DisplaySkuAvailabilities { get; set; }
    }
}
