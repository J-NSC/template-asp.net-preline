namespace api_doc.Infrastructure.Vite;

public sealed class ViteManifestOptions
{
    /// <summary> Ex.: "src/main.ts" </summary>
    public string Entry { get; set; } = "src/main.ts";

    /// <summary> URL do dev server. Ex.: "http://localhost:5173" </summary>
    public string DevServerUrl { get; set; } = "http://localhost:5173";

    /// <summary> Caminho do manifest relativo à webroot. Ex.: "manifest.json" </summary>
    public string ManifestFile { get; set; } = "manifest.json";
}
