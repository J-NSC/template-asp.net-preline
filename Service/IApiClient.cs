using System.Net.Http;

namespace api_doc.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string path, Dictionary<string, string?>? query = null, CancellationToken ct = default);
    Task<List<T>> GetListAsync<T>(string path, Dictionary<string, string?>? query = null, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default);
    Task<bool> PostAsync<TRequest>(string path, TRequest body, CancellationToken ct = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default);
    Task<TResponse?> PostMultipartAsync<TResponse>(string path, MultipartFormDataContent content, CancellationToken ct = default);
    Task<bool> PostMultipartAsync(string path, MultipartFormDataContent content, CancellationToken ct = default);
    Task<bool> DeleteAsync(string path, CancellationToken ct = default);
    HttpClient Http { get; }
}
