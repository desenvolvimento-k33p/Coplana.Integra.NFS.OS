using System.Net;

namespace Coplana.Integracao.NfsOs.Core.Interfaces
{
    public interface IServiceLayerAdapter
    {
        Task<T> Call<T>(string endPoint, HttpMethod method, object obj, string uri = null, string sessionId = null) where T : class;
        Task<CookieContainer> Login();
    }
}
