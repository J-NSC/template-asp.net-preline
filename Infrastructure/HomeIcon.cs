using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure;

[HtmlTargetElement("ui-icon" , TagStructure = TagStructure.NormalOrSelfClosing)]
public class HomeIcon:TagHelper
{
    readonly IWebHostEnvironment _env;
    
    public HomeIcon (IWebHostEnvironment env) => _env = env;
    
    [HtmlAttributeName("name")] public string Name { get; set; } = string.Empty;

    [HtmlAttributeName("class")] public string? Class { get; set; }

    [HtmlAttributeName("stroke")] public string? Stroke { get; set; } 
    [HtmlAttributeName("fill")]   public string? Fill { get; set; }
    [HtmlAttributeName("w")]      public int? Width { get; set; }
    [HtmlAttributeName("h")]      public int? Height { get; set; }
    [HtmlAttributeName("title")]  public string? Title { get; set; }
    
       public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            output.Content.SetHtmlContent("<!-- ui-icon: missing name -->");
            return;
        }

        var path = Path.Combine(_env.WebRootPath, "components/icons", $"{Name}.svg");
        
        Console.WriteLine(path);
        
        if (!File.Exists(path))
        {
            output.Content.SetHtmlContent($"<!-- ui-icon: '{Name}' not found -->");
            return;
        }

        var svg = await File.ReadAllTextAsync(path);

        svg = InjectAttributes(svg, Class, Stroke, Fill, Width, Height, Title);

        output.Content.SetHtmlContent(svg);
    }

    private static string InjectAttributes(
        string svg, string? @class, string? stroke, string? fill, int? w, int? h, string? title)
    {
        var m = Regex.Match(svg, "<svg\\b[^>]*>", RegexOptions.IgnoreCase);
        if (!m.Success) return svg;

        var openTag = m.Value;

        // garante xmlns e viewBox se quiser forçar defaults (opcional)
        // openTag = EnsureAttr(openTag, "xmlns", "http://www.w3.org/2000/svg");

        if (!string.IsNullOrEmpty(@class)) openTag = EnsureAttr(openTag, "class", @class);
        if (w.HasValue) openTag = EnsureAttr(openTag, "width",  w.Value.ToString());
        if (h.HasValue) openTag = EnsureAttr(openTag, "height", h.Value.ToString());
        if (!string.IsNullOrEmpty(stroke)) openTag = EnsureAttr(openTag, "stroke", stroke);
        if (!string.IsNullOrEmpty(fill))   openTag = EnsureAttr(openTag, "fill",   fill);

        if (!string.IsNullOrEmpty(title))
        {
            openTag = EnsureAttr(openTag, "role", "img");
            openTag = EnsureAttr(openTag, "aria-label", title);
        }
        else
        {
            openTag = EnsureAttr(openTag, "aria-hidden", "true");
            openTag = EnsureAttr(openTag, "focusable", "false");
        }

        svg = svg.Remove(m.Index, m.Length).Insert(m.Index, openTag);

        if (!string.IsNullOrEmpty(title))
        {
            var insertAt = m.Index + openTag.Length;
            svg = svg.Insert(insertAt, $"<title>{System.Net.WebUtility.HtmlEncode(title)}</title>");
        }

        return svg;
    }

    private static string EnsureAttr(string openTag, string name, string value)
    {
        // se já tem o atributo, substitui; senão adiciona
        var rx = new Regex($@"\b{name}\s*=\s*""[^""]*""", RegexOptions.IgnoreCase);
        if (rx.IsMatch(openTag))
            return rx.Replace(openTag, $"{name}=\"{System.Net.WebUtility.HtmlEncode(value)}\"");
        return openTag.Insert(openTag.Length - 1, $" {name}=\"{System.Net.WebUtility.HtmlEncode(value)}\"");
    }
}
