using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace api_doc.Service;

public class SwaggerDocumentService
{
    private readonly IHttpClientFactory _cf;

    public SwaggerDocumentService(IHttpClientFactory cf) => _cf = cf;

    public async Task<OpenApiDocument?> GetDocumentAsync(CancellationToken ct = default)
    {
        var http = _cf.CreateClient("SwaggerDoc");
        using var stream = await http.GetStreamAsync("", ct);
        var reader = new OpenApiStreamReader();
        var doc = reader.Read(stream, out var diag);
        return doc;
    }
}
