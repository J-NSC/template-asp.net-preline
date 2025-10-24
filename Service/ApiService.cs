namespace api_doc.Service;

public class ApiService
{
    readonly HttpClient _client;
    
    public ApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<T?> GetAsync<T>(string endPoint)
    {
        var url = await _client.GetFromJsonAsync<T>(endPoint);
        Console.WriteLine(url);
        return url;
    }
}
