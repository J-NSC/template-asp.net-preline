using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Vite;


[HtmlTargetElement("vite", TagStructure = TagStructure.WithoutEndTag)]
public sealed class ViteTagHelper : TagHelper
{
    readonly IWebHostEnvironment _env;
    readonly ViteManifestOptions _opts;
    readonly IViteManifestService _svc;

    public ViteTagHelper(IWebHostEnvironment env, ViteManifestOptions opts, IViteManifestService svc)
    {
        _env = env;
        _opts = opts;
        _svc = svc;
    }

    /// <summary>Entrada do Vite, ex.: "src/main.ts". Se não setado, usa options.Entry</summary>
    [HtmlAttributeName("entry")]
    public string? Entry { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (_env.IsDevelopment())
        {
            var dev = _opts.DevServerUrl.TrimEnd('/');
            var devClient = $"{dev}/@vite/client";
            var entry = Entry ?? _opts.Entry;

            var html =
                $@"<script type=""module"" src=""{devClient}""></script>
                    <script type=""module"" src=""{dev}/{entry}""></script>";

            output.Content.SetHtmlContent(html);
        }
        else
        {
            var entry = Entry ?? _opts.Entry;
            var (jsFile, cssFiles) = _svc.Resolve(entry);

            var sb = new System.Text.StringBuilder();
            foreach (var css in cssFiles.Distinct())
                sb.AppendLine($@"<link rel=""stylesheet"" href=""/{css}"" />");

            if (!string.IsNullOrWhiteSpace(jsFile))
                sb.AppendLine($@"<script type=""module"" src=""/{jsFile}""></script>");

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
