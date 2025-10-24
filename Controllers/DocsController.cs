using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using api_doc.Service;
using api_doc.Models.Docs;
using api_doc.Models.Ui;

public class DocsController : Controller
{
    readonly SwaggerDocumentService _svc;
    public DocsController(SwaggerDocumentService svc) => _svc = svc;

    static string MakeOpId(string path, OperationType method, OpenApiOperation op)
        => (op.OperationId ?? $"{method}-{path}")
           .ToLowerInvariant()
           .Replace("/", "_")
           .Replace("{", "")
           .Replace("}", "")
           .Replace(":", "")
           .Replace(" ", "");

    [HttpGet("docs", Name = "docs_index")]
    public async Task<IActionResult> Index()
    {
        var doc = await _svc.GetDocumentAsync();
        if (doc is null) return View("ErrorLoading");
        ViewData["Breadcrumb"] = Breadcrumbs.Tail(this, "Documentação", Url.Action("Index", "Docs"));

        var byTag = doc.Paths
            .SelectMany(p => p.Value.Operations.Select(op => new {
                Path = p.Key,
                OperationType = op.Key,
                Operation = op.Value,
                Tags = op.Value.Tags?.Select(t => t.Name).DefaultIfEmpty("General") ?? new[] { "General" },
                OpId = MakeOpId(p.Key, op.Key, op.Value)
            }))
            .SelectMany(x => x.Tags.Select(tag => new { tag, x.Path, x.OperationType, x.Operation, x.OpId }))
            .GroupBy(x => x.tag)
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new DocOp(x.tag, x.Path, x.OperationType, x.Operation) { RouteId = x.OpId }).ToList()
            );

        return View((doc, byTag));
    }

    [HttpGet("docs/{tag}/{operationId}", Name = "docs-operation")]
    public async Task<IActionResult> Operation(string tag, string operationId)
    {
        var doc = await _svc.GetDocumentAsync();
        if (doc is null) return NotFound();

        var match = doc.Paths
            .SelectMany(p => p.Value.Operations.Select(op => new {
                Path = p.Key, Method = op.Key, Op = op.Value
            }))
            .FirstOrDefault(x =>
                    string.Equals(x.Op.OperationId, operationId, StringComparison.OrdinalIgnoreCase)
            );

        if (match is null) return NotFound();

        ViewData["Title"] = match.Op.Summary ?? match.Op.OperationId;
        ViewData["Breadcrumb"] = Breadcrumbs.Tail(this, "operação").Parent(this, "Documentação",  Url.Action("Index", "Docs"));

        return View((doc, (object)match, tag));
    }
}
