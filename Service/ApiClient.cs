using System.Text.Json;
using api_doc.Option;
using Microsoft.Extensions.Options;

namespace api_doc.Services;

public class ApiClient : IApiClient
{
    readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpClient Http { get; }

    public ApiClient(HttpClient http, IOptions<ApiOption> opt)
    {
        Http = http;
        var cfg = opt.Value;

        Http.BaseAddress ??= new Uri(cfg.BaseUrl);
        Http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        // if (!string.IsNullOrWhiteSpace(cfg.ApiKey))
        //     Http.DefaultRequestHeaders.Add("X-API-KEY", cfg.ApiKey);
    }

    static string BuildPath(string path, Dictionary<string, string?>? query)
    {
        if (query is null || query.Count == 0) return path.TrimStart('/');
        var qs = string.Join("&", query.Where(kv => !string.IsNullOrEmpty(kv.Value))
                                       .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
        return $"{path.TrimStart('/')}?{qs}";
    }

    static async Task<string> ReadErrorAsync(HttpResponseMessage resp)
        => $"{(int)resp.StatusCode} {resp.ReasonPhrase} :: {await resp.Content.ReadAsStringAsync()}";

    public async Task<T?> GetAsync<T>(string path, Dictionary<string, string?>? query = null, CancellationToken ct = default)
    {
        var url = BuildPath(path, query);
        var resp = await Http.GetAsync(url, ct);
        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<T>(_json, ct);

        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }

    public async Task<List<T>> GetListAsync<T>(string path, Dictionary<string, string?>? query = null, CancellationToken ct = default)
        => await GetAsync<List<T>>(path, query, ct) ?? new();

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default)
    {
        var resp = await Http.PostAsJsonAsync(path, body, _json, ct);
        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<TResponse>(_json, ct);

        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }

    public async Task<bool> PostAsync<TRequest>(string path, TRequest body, CancellationToken ct = default)
    {
        var resp = await Http.PostAsJsonAsync(path, body, _json, ct);
        if (resp.IsSuccessStatusCode) return true;
        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }
    
    public async Task<TResponse?> PostMultipartAsync<TResponse>(string path, MultipartFormDataContent content, CancellationToken ct = default)
    {
        using var resp = await Http.PostAsync(path, content, ct); // NÃO usa PostAsJsonAsync
        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<TResponse>(_json, ct);

        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }

    public async Task<bool> PostMultipartAsync(string path, MultipartFormDataContent content, CancellationToken ct = default)
    {
        using var resp = await Http.PostAsync(path, content, ct);
        if (resp.IsSuccessStatusCode) return true;

        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default)
    {
        var resp = await Http.PutAsJsonAsync(path, body, _json, ct);
        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadFromJsonAsync<TResponse>(_json, ct);

        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }

    public async Task<bool> DeleteAsync(string path, CancellationToken ct = default)
    {
        var resp = await Http.DeleteAsync(path, ct);
        if (resp.IsSuccessStatusCode) return true;
        throw new HttpRequestException(await ReadErrorAsync(resp), null, resp.StatusCode);
    }
}
