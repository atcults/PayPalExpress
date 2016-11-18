using Newtonsoft.Json;

namespace PayPal.Model
{
    public class RedirectUrls
    {
        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }
        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }
    }
}