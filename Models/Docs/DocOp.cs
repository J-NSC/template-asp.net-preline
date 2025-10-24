using Microsoft.OpenApi.Models;

namespace api_doc.Models.Docs;

public sealed record DocOp(string Tag, string Path, OperationType OperationType, OpenApiOperation Operation)
{
    public string RouteId { get; set; } = "";
}
