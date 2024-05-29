using Coplana.Integracao.NfsOs.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Coplana.Integracao.NfsOs.Domain.Configuration;
using Coplana.Integracao.NfsOs.Core.Models;
using Coplana.Integracao.NfsOs.Core.Models.Response;
using System.Text.Json.Serialization;

namespace Coplana.Integracao.NfsOs.Core.Adapters
{
    public class ServiceLayerAdapter : IServiceLayerAdapter
    {
        private readonly string _cookie = "B1SESSION";
        private readonly string _type = "application/json";
        private readonly string _header = "Content-Type";
        private readonly string _baseUrl = "/b1s/v1";
        private readonly ILogger<ServiceLayerAdapter> _logger;
        private IOptions<Configuration> _configurations;
        private CookieContainer _httpCookieContainer = null;

        public ServiceLayerAdapter(ILogger<ServiceLayerAdapter> logger, IOptions<Configuration> configurations)
        {
            _logger = logger;
            _configurations = configurations;
        }

        public async Task<T> Call<T>(string endPoint, HttpMethod method, object obj = null, string uri = null, string sessionId = null) where T : class
        {
            var cookieContainer = await Login();

            if (cookieContainer == null)
                throw new Exception("Não foi possível efetuar login");

            var c = cookieContainer.GetAllCookies();

            var indexPrimeiroCaracter = endPoint.IndexOf("/");
            if (indexPrimeiroCaracter == 0)
                endPoint = endPoint.Remove(0, 1);

            var baseUrl = new Uri($"{uri}{_baseUrl}/");

            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                CookieContainer = _httpCookieContainer
            };

            using (clientHandler)
            {
                using (var client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = baseUrl;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    var json = JsonSerializer.Serialize(obj);

                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        using (var httpRequestMensage = new HttpRequestMessage(method, endPoint))
                        {
                            if (obj != null)
                                httpRequestMensage.Content = stringContent;

                            var response = await client.SendAsync(httpRequestMensage);
                            var statusSuccess = $"{HttpStatusCode.Created}|{HttpStatusCode.OK}";

                            if (!statusSuccess.Contains(response.StatusCode.ToString()))
                                _logger.LogError($"responseStatus={response.StatusCode} - [{method}] - [{baseUrl.AbsoluteUri}Login] payload={json}");
                            else
                                _logger.LogInformation($"responseStatus={response.StatusCode} - [{method}] - [{baseUrl.AbsoluteUri}Login] payload={json}");

                            var content = await response.Content.ReadAsStringAsync();

                            var retZerado = default(T);

                            if (string.IsNullOrEmpty(content))
                                return retZerado;
                           

                            var contentToObject = JsonSerializer.Deserialize<T>(content);
                            return contentToObject;
                        }
                    }
                }
            }
        }

        public async Task<CookieContainer> Login()
        {
            try
            {
                var serviceLayerConfiguration = _configurations.Value.ServiceLayer;
                var baseUrl = new Uri($"{serviceLayerConfiguration.Uri}{_baseUrl}/");
                var login = new LoginPost()
                {
                    CompanyDB = Criptografia.Instancia.Descriptografar(serviceLayerConfiguration.CompanyDB),
                    Password = Criptografia.Instancia.Descriptografar(serviceLayerConfiguration.Password),
                    UserName = Criptografia.Instancia.Descriptografar(serviceLayerConfiguration.UserName)
                };

                if (_httpCookieContainer == null)
                    _httpCookieContainer = new CookieContainer();

                var clientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                    CookieContainer = _httpCookieContainer
                };

                var client = new HttpClient(clientHandler);
                var cookieContainer = _httpCookieContainer.GetCookies(baseUrl);
                var dataString = string.Empty;
                DateTime? dataValidaSessao = null;


                if (cookieContainer["DataValidaSessao"] != null)
                {
                    dataString = cookieContainer["DataValidaSessao"].Value;
                    dataValidaSessao = DateTime.ParseExact(dataString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                if (dataValidaSessao.HasValue && dataValidaSessao.Value >= DateTime.Now)
                    return _httpCookieContainer;

                _httpCookieContainer = new CookieContainer();

                using (clientHandler)
                {
                    using (client)
                    {
                        client.BaseAddress = baseUrl;
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var json = JsonSerializer.Serialize(login);

                        using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            using (var httpRequestMensage = new HttpRequestMessage(HttpMethod.Post, "Login"))
                            {
                                httpRequestMensage.Content = stringContent;

                                var response = await client.SendAsync(httpRequestMensage);

                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    var ret = await response.Content.ReadAsStringAsync();
                                    _logger.LogInformation($"responseStatus={response.StatusCode} - [{HttpMethod.Post}] - [{baseUrl.AbsoluteUri}Login] payload={json}");
                                    throw new Exception($"Erro ao efetuar login: {ret}");
                                }

                                var restResponseCookies = _httpCookieContainer.GetAllCookies().ToList();

                                var content = await response.Content.ReadAsStringAsync();
                                var responseData = JsonSerializer.Deserialize<ServiceLayerResponse>(content);

                                var dataSessao = DateTime.Now.AddMinutes(responseData.SessionTimeout);
                                _httpCookieContainer.Add(baseUrl, new Cookie("DataValidaSessao", dataSessao.ToString("yyyy-MM-dd HH:mm:ss")));
                                _httpCookieContainer.Add(baseUrl, new Cookie("B1SESSION", responseData.SessionId));

                                return _httpCookieContainer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return null;
            }
        }
    }
}
