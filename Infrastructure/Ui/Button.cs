using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-button", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class UiButtonTagHelper : TagHelper
{
    [HtmlAttributeName("type")] public string Type { get; set; } = "button";

    [HtmlAttributeName("color")] public string Color { get; set; } = "primary";

    [HtmlAttributeName("size")] public string Size { get; set; } = "medium";

    [HtmlAttributeName("disabled")] public bool Disabled { get; set; } = false;

    [HtmlAttributeName("icon")] public string? IconClass { get; set; }

    [HtmlAttributeName("icon-position")] public string IconPosition { get; set; } = "left";

    [HtmlAttributeName("full-width")] public bool FullWidth { get; set; } = false;

    [HtmlAttributeName("label")] public string? Label { get; set; }

    [HtmlAttributeName("icon-svg")] public string? IconSvg { get; set; }
    

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "button";
        output.TagMode = TagMode.StartTagAndEndTag;

        var @base = "btn-base";

        var color = Color.ToLowerInvariant();
        var colorClasses = color switch
        {
            "primary"   => "btn-primary",
            "secondary" => "btn-secondary",
            "success"   => "btn-success",
            "danger"    => "btn-danger",
            "warning"   => "btn-warning",
            "light"     => "btn-light",
            "dark"      => "btn-dark",
            _           => "btn-primary"
        };

        var size = Size.ToLowerInvariant();
        var sizeClasses = size switch
        {
            "small" => "btn-sm",
            "large" => "btn-lg",
            _       => "btn-md"
        };

        var fullWidth = FullWidth ? "btn-block" : string.Empty;

        var justify = string.IsNullOrWhiteSpace(IconClass) && string.IsNullOrWhiteSpace(IconSvg)
            ? "justify-center"
            : string.Empty;

        var extraClass = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value?.ToString();
        if (!string.IsNullOrWhiteSpace(extraClass))
            output.Attributes.RemoveAll("class");

        
        var finalClasses = string.Join(" ",
            @base, colorClasses, sizeClasses, fullWidth, justify, extraClass ?? string.Empty
        ).Trim();

        output.Attributes.SetAttribute("class", finalClasses);
        if (Disabled) output.Attributes.SetAttribute("disabled", "disabled");
        output.Attributes.SetAttribute("class", finalClasses);

        var childContent = await output.GetChildContentAsync();
        var text = !string.IsNullOrWhiteSpace(Label) ? Label : childContent.GetContent();

        var hasIcon = !string.IsNullOrWhiteSpace(IconClass) || !string.IsNullOrWhiteSpace(IconSvg);

        string iconLeft = string.Empty, iconRight = string.Empty;
        if (hasIcon)
        {
            var iconHtml = !string.IsNullOrWhiteSpace(IconSvg)
                ? IconSvg! // SVG bruto
                : $@"<i class=""w-4 h-4 {IconClass}""></i>"; // classe do ícone

            if (IconPosition.Equals("right", StringComparison.OrdinalIgnoreCase))
                iconRight = iconHtml;
            else
                iconLeft = iconHtml;
        }

        var html = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(iconLeft))  html.Append(iconLeft).Append(' ');
        html.Append(string.IsNullOrWhiteSpace(text) ? "Button" : text);
        if (!string.IsNullOrWhiteSpace(iconRight)) html.Append(' ').Append(iconRight);

        output.Content.SetHtmlContent(html.ToString());
    }
}
