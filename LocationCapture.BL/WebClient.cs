using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocationCapture.BL
{
    public class WebClient : IWebClient
    {
        private readonly ILoggingService _loggingService;

        public WebClient(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetStringAsync(requestUri);
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
            where TResponse : class
        {
            using (var httpClient = new HttpClient())
            {
                return await InvokeHttpActionAsync<object, TResponse>(url,
                    (uri, content) => httpClient.GetAsync(uri));
            }
        }

        public async Task<TResponse> PostAsync<TPayload, TResponse>(string url, TPayload payload, string bearer = null)
            where TPayload : class
            where TResponse : class
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(bearer))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearer}");
                }

                return await InvokeHttpActionAsync<TPayload, TResponse>(url, httpClient.PostAsync, payload);
            }
        }

        public async Task<TResponse> PutAsync<TPayload, TResponse>(string url, TPayload payload)
            where TPayload : class
            where TResponse : class
        {
            using (var httpClient = new HttpClient())
            {
                return await InvokeHttpActionAsync<TPayload, TResponse>(url, httpClient.PutAsync, payload);
            }
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string url)
            where TResponse : class
        {
            using (var httpClient = new HttpClient())
            {
                return await InvokeHttpActionAsync<object, TResponse>(url,
                    (uri, content) => httpClient.DeleteAsync(uri));
            }
        }

        private async Task<TResponse> InvokeHttpActionAsync<TPayload, TResponse>(string url,
            Func<Uri, HttpContent, Task<HttpResponseMessage>> httpAction,
            TPayload payload = null)
                where TPayload : class
                where TResponse : class
        {
            var requestUri = new Uri(url);
            var json = payload == null ? string.Empty : JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpAction(requestUri, content);
            if(!response.IsSuccessStatusCode)
            {
                var details = await response.Content.ReadAsStringAsync();
                _loggingService.Warning("HTTP request failed. Details: {Details}", details);
                throw new HttpRequestException($"HTTP request failed: {response.ReasonPhrase}");
            }
            string responJsonText = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResponse>(responJsonText);
            return result;
        }
    }
}
