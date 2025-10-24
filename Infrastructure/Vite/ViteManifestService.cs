using System.Text.Json;

namespace api_doc.Infrastructure.Vite;

public sealed class ViteManifestService : IViteManifestService
{
    readonly IWebHostEnvironment _env;
    readonly ViteManifestOptions _opts;

    public ViteManifestService(IWebHostEnvironment env, ViteManifestOptions opts)
    {
        _env = env;
        _opts = opts;
    }

    public (string? jsFile, IReadOnlyList<string> cssFiles) Resolve(string entry)
    {
        var webRoot = _env.WebRootPath ?? string.Empty;
        var manifestPath = Path.Combine(webRoot, _opts.ManifestFile);
        if (!File.Exists(manifestPath))
            return (null, Array.Empty<string>());

        using var doc = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var root = doc.RootElement;

        if (!root.TryGetProperty(entry, out var entryEl))
            return (null, Array.Empty<string>());

        var jsFile = entryEl.TryGetProperty("file", out var f) ? f.GetString() : null;
        var css = new List<string>();

        if (entryEl.TryGetProperty("css", out var cssArr))
            foreach (var c in cssArr.EnumerateArray())
                AddIfNotNull(css, c.GetString());

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        ResolveImportsCss(root, entryEl, css, visited);

        return (jsFile, css);
    }

    static void ResolveImportsCss(JsonElement root, JsonElement node, List<string> css, HashSet<string> visited)
    {
        if (!node.TryGetProperty("imports", out var importsArr))
            return;

        foreach (var imp in importsArr.EnumerateArray())
        {
            var key = imp.GetString();
            if (string.IsNullOrWhiteSpace(key) || visited.Contains(key))
                continue;

            visited.Add(key);
            if (!root.TryGetProperty(key, out var impNode))
                continue;

            if (impNode.TryGetProperty("css", out var cssArr))
                foreach (var c in cssArr.EnumerateArray())
                    AddIfNotNull(css, c.GetString());

            ResolveImportsCss(root, impNode, css, visited);
        }
    }

    private static void AddIfNotNull(List<string> list, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            list.Add(value!);
    }
}
