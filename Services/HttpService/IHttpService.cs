namespace StockhubWeb.Services.HttpService
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, object data);
        Task<T> PutAsync<T>(string url, object data);
        Task<bool> DeleteAsync(string url);
        Task<T> SendAsync<T>(HttpRequestMessage request);
        void SetBaseUrl(string baseUrl);
        void SetTimeout(TimeSpan timeout);
        void AddHeader(string key, string value);
        void RemoveHeader(string key);
    }
}