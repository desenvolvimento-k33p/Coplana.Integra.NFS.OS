using RestSharp;

namespace Coplana.Integracao.NfsOs.Core.Interfaces
{
    public interface IHttpAdapter
    {
        Task<T> Call<T>(
            HttpMethod method,
            string endPoint,
            object obj = null,
            string uri = null) where T : class;
        Task<RestResponse<T>> ExecuteRequestAsync<T>(RestClient client, RestRequest request);


        Task<T> CallProduct<T>(
           HttpMethod method,
           string endPoint,
           object obj = null,
           string uri = null) where T : class;

    }
}
