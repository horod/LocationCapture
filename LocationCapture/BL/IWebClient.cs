using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public interface IWebClient
    {
        Task<string> GetStringAsync(string requestUri);

        Task<TResponse> GetAsync<TResponse>(string url)
            where TResponse : class;

        Task<TResponse> PostAsync<TPayload, TResponse>(string url, TPayload payload)
            where TPayload : class
            where TResponse : class;

        Task<TResponse> PutAsync<TPayload, TResponse>(string url, TPayload payload)
            where TPayload : class
            where TResponse : class;

        Task<TResponse> DeleteAsync<TResponse>(string url)
            where TResponse : class;
    }
}
