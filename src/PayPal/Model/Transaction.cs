using Newtonsoft.Json;

namespace PayPal.Model
{
    public class Transaction
    {
        [JsonProperty("amount")]
        public Amount Amount { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}