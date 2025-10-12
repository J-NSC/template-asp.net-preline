namespace api_doc.Infrastructure.Vite;

public interface IViteManifestService
{
    /// <summary>Retorna (jsFile, cssFiles[]) para o entry informado, resolvendo imports recursivos.</summary>
    (string? jsFile, IReadOnlyList<string> cssFiles) Resolve(string entry);
}
