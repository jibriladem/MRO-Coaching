using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MROCoatching.DataObjects.Models
{
    public class CustomHttp
    {
        public HttpResponseMessage PostJson<Tpara>(string baseAddress, string apiAddress, Tpara para, string token, string authenticationHeaderType, Dictionary<string, string> cookies, Dictionary<string, string> headers)
        {
            try
            {
                var cookieContainer = new CookieContainer();

                using (var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true, CookieContainer = cookieContainer })
                {
                    using (var client = new HttpClient(handler))
                    {
                        //Add Cookie Values
                        if (cookies != null)
                        {
                            foreach (var cookie in cookies)
                            {
                                if (!String.IsNullOrEmpty(cookie.Value))
                                {
                                    cookieContainer.Add(new Uri(baseAddress), new Cookie(cookie.Key, cookie.Value));
                                }
                            }
                        }
                        ////Trust all certificates
                        ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        client.BaseAddress = new Uri(baseAddress);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (authenticationHeaderType != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationHeaderType, token);
                        }
                        //Add Header Values
                        if (headers != null)
                        {
                            foreach (var header in headers)
                            {
                                if (!String.IsNullOrEmpty(header.Value))
                                {
                                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                                }
                            }
                        }
                        client.Timeout = TimeSpan.FromMinutes(60);
                        HttpResponseMessage response = client.PostAsync(apiAddress, new StringContent(JsonConvert.SerializeObject(para,
                               Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                }).ToString(),

                      Encoding.UTF8, "application/json")).Result;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
