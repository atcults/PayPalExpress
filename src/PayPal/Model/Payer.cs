using Newtonsoft.Json;

namespace PayPal.Model
{
    public class Payer
    {
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }
    }
}