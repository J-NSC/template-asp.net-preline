using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace api_doc.Infrastructure.Http;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    readonly IHttpContextAccessor _http;
    ILogger<AuthHeaderHandler> _logger;

    public AuthHeaderHandler(IHttpContextAccessor http, ILogger<AuthHeaderHandler> logger)
    {
        _http = http;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var ctx = _http.HttpContext;
        var token = ctx?.Request.Cookies["access_token"];
        _logger.LogInformation("AuthHeaderHandler: token presente? {HasToken}", !string.IsNullOrWhiteSpace(token));
        
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
