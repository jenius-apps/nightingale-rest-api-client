using Newtonsoft.Json;
using System;

namespace Nightingale.StoreHelpers.Models
{
    public class CollectionData
    {
        [JsonProperty("acquiredDate")]
        public DateTime AcquiredDate { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}