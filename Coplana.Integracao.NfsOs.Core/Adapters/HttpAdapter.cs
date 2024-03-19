﻿using Coplana.Integracao.NfsOs.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Coplana.Integracao.NfsOs.Domain.Configuration;

namespace Coplana.Integracao.NfsOs.Core.Adapters
{
    public class HttpAdapter : IHttpAdapter
    {
        private readonly ILogger<HttpAdapter> _logger;

        private readonly string MAGIS_AUTHORIZATION_KEY = "APIKey";
        private readonly string MAGIS_AUTHORIZATION_VALUE;

        public HttpAdapter(ILogger<HttpAdapter> logger, IOptions<Configuration> configurations)
        {
            _logger = logger;
            MAGIS_AUTHORIZATION_VALUE = configurations.Value.CoplanaHttp.Key;
        }

        public async Task<T> Call<T>(
            HttpMethod method,
            string endPoint,
            object obj = null,
            string uri = null) where T : class
        {
            try
            {
                var clientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Clear();


                    if (endPoint.Contains("consultar"))
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    else
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));


                    client.DefaultRequestHeaders.Add(MAGIS_AUTHORIZATION_KEY, MAGIS_AUTHORIZATION_VALUE);

                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    using (var httpRequestMensage = new HttpRequestMessage(method, endPoint))
                    {
                        if (obj != null)
                        {
                            if (!obj.ToString().Contains("xml"))
                            {
                                var json1 = JsonSerializer.Serialize(obj);
                                var stringContent = new StringContent(json1, Encoding.UTF8, "text/plain");

                                if (!endPoint.Contains("invoices/queues/read"))
                                    httpRequestMensage.Content = stringContent;

                            }
                            else
                            {
                                var stringContent = new StringContent(obj.ToString(), Encoding.UTF8, "application/xml");
                                httpRequestMensage.Content = stringContent;
                            }
                        }

                        var response = await client.SendAsync(httpRequestMensage);
                        var content = await response.Content.ReadAsStringAsync();
                        var retDefault = default(T);
                        var statusSuccess = $"{HttpStatusCode.Created}|{HttpStatusCode.OK}";

                        if (!statusSuccess.Contains(response.StatusCode.ToString()))
                        {
                            _logger.LogError($"responseStatus={response.StatusCode} -  payload={JsonSerializer.Serialize(obj)}");
                            //var contentToObject = JsonSerializer.Deserialize<T>(content);

                            return retDefault;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(content) || (endPoint.Contains("products") && method.ToString().Equals("POST")))
                            {
                                return retDefault;
                            }
                            // return retDefault;
                            var contentToObject = JsonSerializer.Deserialize<T>(content);
                            _logger.LogInformation($"responseStatus={response.StatusCode} - payload={JsonSerializer.Serialize(obj)}");

                            return contentToObject;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> CallProduct<T>(
            HttpMethod method,
            string endPoint,
            object obj = null,
            string uri = null) where T : class
        {
            try
            {
                var clientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Clear();


                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    client.DefaultRequestHeaders.Add(MAGIS_AUTHORIZATION_KEY, MAGIS_AUTHORIZATION_VALUE);

                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    using (var httpRequestMensage = new HttpRequestMessage(method, endPoint))
                    {
                        if (obj != null)
                        {
                            if (!obj.ToString().Contains("xml"))
                            {
                                var json1 = JsonSerializer.Serialize(obj);
                                var stringContent = new StringContent(json1, Encoding.UTF8, "application/json");
                                httpRequestMensage.Content = stringContent;
                            }
                            else
                            {
                                var stringContent = new StringContent(obj.ToString(), Encoding.UTF8, "application/xml");
                                httpRequestMensage.Content = stringContent;
                            }
                        }

                        var response = await client.SendAsync(httpRequestMensage);
                        var content = await response.Content.ReadAsStringAsync();
                        var retDefault = default(T);
                        var statusSuccess = $"{HttpStatusCode.Created}|{HttpStatusCode.OK}";

                        if (!statusSuccess.Contains(response.StatusCode.ToString()))
                        {
                            _logger.LogError($"responseStatus={response.StatusCode} -  payload={JsonSerializer.Serialize(obj)}");
                            var contentToObject = JsonSerializer.Deserialize<T>(content);

                            return contentToObject;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(content))// || (endPoint.Contains("products") && method.ToString().Equals("POST")))
                            {
                                return retDefault;
                            }
                            // return retDefault;
                            var contentToObject = JsonSerializer.Deserialize<T>(content);
                            _logger.LogInformation($"responseStatus={response.StatusCode} - payload={JsonSerializer.Serialize(obj)}");

                            return contentToObject;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RestResponse<T>> ExecuteRequestAsync<T>(RestClient client, RestRequest request)
    => await client.ExecuteAsync<T>(request);
    }
}
