using System;
using Microsoft.Extensions.Configuration;

namespace PayPal
{
    public class ConfigurationProvider
    {
        private static readonly Lazy<IConfigurationRoot> Lazy = new Lazy<IConfigurationRoot>(() =>
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("paypal.json", false);
            return configurationBuilder.Build();
        });

        private static IConfigurationRoot Instance => Lazy.Value;

        private ConfigurationProvider()
        {
        }

        public static string GetServiceUrl(string pathService)
        {
            var serviceUrl = IsSandBox() ? Instance["SandBox:EndPoint"] : Instance["Live:EndPoint"];
            return serviceUrl + pathService;
        }

        public static string GetClientId()
        {
            var clientId = IsSandBox() ? Instance["SandBox:ClientId"] : Instance["Live:ClientId"];
            return clientId;
        }

        public static string GetClientSecret()
        {
            var clientSecret = IsSandBox() ? Instance["SandBox:ClientSecret"] : Instance["Live:ClientSecret"];
            return clientSecret;
        }

        public static bool IsSandBox()
        {
            return bool.Parse(Instance["UseSendBox"]);
        }

        public static string GetSuccessUrl()
        {
            return Instance["SuccessUrl"];
        }

        public static string GetCancelUrl()
        {
            return Instance["CancelUrl"];
        }
    }
}