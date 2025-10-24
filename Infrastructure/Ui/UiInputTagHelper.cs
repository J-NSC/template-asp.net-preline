using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

[HtmlTargetElement("ui-input", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class UiInputTagHelper : TagHelper
{
    // injeta ViewContext automaticamente
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    [HtmlAttributeName("label")] public string? Label { get; set; }
    [HtmlAttributeName("type")] public string Type { get; set; } = "text";
    [HtmlAttributeName("id")] public string? Id { get; set; }
    [HtmlAttributeName("name")] public string? Name { get; set; }
    [HtmlAttributeName("value")] public string? Value { get; set; }
    [HtmlAttributeName("placeholder")] public string? Placeholder { get; set; }
    [HtmlAttributeName("disabled")] public bool Disabled { get; set; }
    [HtmlAttributeName("readonly")] public bool Readonly { get; set; }
    [HtmlAttributeName("autocomplete")] public string? Autocomplete { get; set; }
    [HtmlAttributeName("container-class")] public string ContainerClass { get; set; } = "w-full max-w-sm";
    [HtmlAttributeName("class")] public string? ExtraInputClass { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", ContainerClass);

        var sb = new StringBuilder();

        var fieldName = Name ?? Id ?? "";
        var fieldId   = Id   ?? Name ?? "";
        var errorMessage = "";

        if (ViewContext?.ViewData?.ModelState is { } ms && !string.IsNullOrEmpty(fieldName))
        {
            if (ms.TryGetValue(fieldName, out var entry) && entry.Errors.Count > 0)
                errorMessage = entry.Errors[0].ErrorMessage;
            else
            {
                var pascal = char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
                if (ms.TryGetValue(pascal, out var entry2) && entry2.Errors.Count > 0)
                    errorMessage = entry2.Errors[0].ErrorMessage;
            }
        }

        if (!string.IsNullOrEmpty(Label))
            sb.AppendLine($"<label for=\"{fieldId}\" class=\"block text-sm font-medium mb-2 dark:text-white\">{Label}</label>");

        var baseInputClass =
            "py-2.5 sm:py-3 px-4 block w-full rounded-lg border bg-white text-gray-900 " +
            "focus:border-blue-500 focus:ring-blue-500 focus:outline-none " +
            "disabled:opacity-50 disabled:pointer-events-none " +
            "dark:bg-neutral-900 dark:border-neutral-700 dark:text-neutral-100 dark:placeholder-neutral-500 dark:focus:ring-neutral-600";

        baseInputClass += string.IsNullOrEmpty(errorMessage) ? " border-gray-200" : " border-red-500 dark:border-rose-500";
        if (!string.IsNullOrWhiteSpace(ExtraInputClass)) baseInputClass += " " + ExtraInputClass;

        sb.Append("<input");
        sb.Append($" type=\"{Type}\"");
        if (!string.IsNullOrEmpty(fieldId)) sb.Append($" id=\"{fieldId}\"");
        if (!string.IsNullOrEmpty(Name))    sb.Append($" name=\"{Name}\"");
        if (!string.IsNullOrEmpty(Value))   sb.Append($" value=\"{Value}\"");
        if (!string.IsNullOrEmpty(Placeholder)) sb.Append($" placeholder=\"{Placeholder}\"");
        if (!string.IsNullOrEmpty(Autocomplete)) sb.Append($" autocomplete=\"{Autocomplete}\"");
        if (Disabled) sb.Append(" disabled");
        if (Readonly) sb.Append(" readonly");
        sb.Append($" class=\"{baseInputClass}\"");
        sb.Append(" />");

        if (!string.IsNullOrEmpty(errorMessage))
            sb.AppendLine($"<p class=\"mt-1 text-sm text-red-600 dark:text-rose-400\">{errorMessage}</p>");

        output.Content.SetHtmlContent(sb.ToString());
    }
}
