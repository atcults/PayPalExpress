using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayPal.Model;

namespace PayPal
{
    public static class RestClient
    {
        private static string _accessToken = "";
        private static DateTime _tokenObtainedTime = DateTime.MinValue;
        /*
	    *   Purpose: 	Gets the access token from PayPal
	    *   Inputs:     n/a
	    *   Returns:    access token
	    *
	    */
        private static async Task GetAccessToken()
        {
            var serviceUrl = ConfigurationProvider.GetServiceUrl("v1/oauth2/token");

            var clientId = ConfigurationProvider.GetClientId();
            var clientSecret = ConfigurationProvider.GetClientSecret();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(clientId + ":" + clientSecret));
            httpWebRequest.Headers["AcceptLanguage"] = "en_US";
            httpWebRequest.Method = "POST";

            const string data = "grant_type=client_credentials";

            var requestStream = await Task.Factory.FromAsync(httpWebRequest.BeginGetRequestStream, httpWebRequest.EndGetRequestStream, null);

            using (var streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
            }

            string result;

            using (var response = await httpWebRequest.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            var jObject = JObject.Parse(result);
            _accessToken = (string)jObject["access_token"];
            _tokenObtainedTime = DateTime.Now;
        }

        /*
        *   Purpose: 	Gets the PayPal approval URL to redirect the user to.
        *   
        *   Inputs:     access_token (The access token received from PayPal)
        *   Returns:    approval URL
        */
        public static string GetApprovalUrl(ExpressCheckoutPaymentData data)
        {
            var payload = JsonConvert.SerializeObject(data, Formatting.Indented);

            var response = ExecuteRequest("v1/payments/payment", "POST", payload);

            response.Wait();

            var responseObject = JObject.Parse(response.Result);

            var approvalUrl = "";

            // parse out the approval_url link
            foreach (var link in responseObject["links"])
            {
                if ((string) link["rel"] != "approval_url") continue;
                approvalUrl = (string)link["href"];
                break;
            }

            return approvalUrl;
        }

        private static async Task<string> ExecuteRequest(string url, string method, string body)
        {
            var serviceUrl = ConfigurationProvider.GetServiceUrl(url);

            if (_tokenObtainedTime < DateTime.Now.AddMinutes(-10))
            {
                await GetAccessToken();
            }


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers["Authorization"] = "Bearer " + _accessToken;
            httpWebRequest.Headers["AcceptLanguage"] = "en_US";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;

            if (!method.Equals("GET"))
            {
                httpWebRequest.Headers["Content-Length"] = body.Length.ToString();

                using (var requestStream = await httpWebRequest.GetRequestStreamAsync())
                {
                    using (var streamWriter = new StreamWriter(requestStream))
                    {
                        streamWriter.Write(body);
                    }
                    requestStream.Dispose();
                }
            }

            string result;


            using (var response = await httpWebRequest.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            return result;
        }

    }
}
