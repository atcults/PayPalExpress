using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayPal.Model;

namespace LibCloud.Controllers
{
    [Route("api/[controller]")]
    public class PayPalExpress : Controller
    {
        private readonly ILogger _logger;

        public PayPalExpress(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("PayPalExpress");
        }

        [HttpGet]
        public string Index()
        {
            _logger.LogDebug("Getting paypal approval");

            var data = new ExpressCheckoutPaymentData
            {
                Intent = "sale",
                Payer = new Payer
                {
                    PaymentMethod = "paypal"
                },
                Transactions = new[]
                {
                    new Transaction
                    {
                        Amount = new Amount
                        {
                            Currency = "USD",
                            Total = "10"
                        },
                        Description = "New Sale"
                    }
                },
                RedirectUrls = new RedirectUrls
                {
                    CancelUrl = "http://sanelib.com/api/cancel",
                    ReturnUrl = "http://sanelib.com/api/success"
                }
            };

            var approvalUrl = PayPal.RestClient.GetApprovalUrl(data);

            return approvalUrl;
        }
        
    }
}
