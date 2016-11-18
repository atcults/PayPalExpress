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

        public static IConfigurationRoot Instance => Lazy.Value;

        private ConfigurationProvider()
        {
        }
    }
}