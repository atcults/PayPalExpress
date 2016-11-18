using System.Collections.Generic;
using Newtonsoft.Json;

namespace PayPal.Model
{
    public class ExpressCheckoutPaymentData
    {
        [JsonProperty("transactions")]
        public IList<Transaction> Transactions { get; set; }

        [JsonProperty("payer")]
        public Payer Payer { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("redirect_urls")]
        public RedirectUrls RedirectUrls { get; set; }
    }
}