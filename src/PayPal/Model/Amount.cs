using Newtonsoft.Json;

namespace PayPal.Model
{
    public class Amount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("total")]
        public string Total { get; set; }
    }
}