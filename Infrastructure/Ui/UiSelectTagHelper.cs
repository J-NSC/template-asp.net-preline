using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-select", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class UiSelectTagHelper : TagHelper
{
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }
    [HtmlAttributeName("x-model")] public string? XModel { get; set; }

    [HtmlAttributeName("name")] public string? Name { get; set; }
    [HtmlAttributeName("id")] public string? Id { get; set; }
    [HtmlAttributeName("label")] public string? Label { get; set; }
    [HtmlAttributeName("disabled")] public bool Disabled { get; set; } = false;
    [HtmlAttributeName("full-width")] public bool FullWidth { get; set; } = true;
    [HtmlAttributeName("placeholder")] public string? Placeholder { get; set; }
    [HtmlAttributeName("items")] public IEnumerable<string>? Items { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var sb = new StringBuilder();

        var fieldName = Name ?? Id ?? "";
        var fieldId = Id ?? Name ?? "";
        var errorMessage = "";

        // 🔍 tenta achar o erro no ModelState (camelCase e PascalCase)
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

        // label
        if (!string.IsNullOrWhiteSpace(Label))
        {
            sb.AppendLine($@"<label for=""{fieldId}"" class=""block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300"">{Label}</label>");
        }

        var widthClass = FullWidth ? "w-full" : string.Empty;

        // classe base + destaque em caso de erro
        var baseSelectClass =
            $"py-3 px-4 pe-9 block {widthClass} rounded-lg text-sm " +
            "bg-gray-100 border-transparent focus:border-blue-500 focus:ring-blue-500 disabled:opacity-50 disabled:pointer-events-none " +
            "dark:bg-neutral-700 dark:border-transparent dark:text-neutral-400 dark:focus:ring-neutral-600";

        if (!string.IsNullOrEmpty(errorMessage))
            baseSelectClass += " border-red-500 dark:border-rose-500";

        sb.AppendLine($@"<select id=""{fieldId}"" name=""{Name}"" class=""{baseSelectClass}"" {(Disabled ? "disabled" : string.Empty)} {(string.IsNullOrEmpty(XModel) ? "" : $@"x-model=""{XModel}""")}>");

        if (!string.IsNullOrWhiteSpace(Placeholder))
        {
            sb.AppendLine($@"<option value="""" selected disabled>{Placeholder}</option>");
        }

        if (Items != null)
        {
            foreach (var item in Items)
            {
                sb.AppendLine($@"<option value=""{item}"">{item}</option>");
            }
        }
        else
        {
            // permite conteúdo manual dentro do <ui-select>
            var childContent = await output.GetChildContentAsync();
            sb.AppendLine(childContent.GetContent());
        }

        sb.AppendLine("</select>");

        // mensagem de erro automática
        if (!string.IsNullOrEmpty(errorMessage))
        {
            sb.AppendLine($@"<p class=""mt-1 text-sm text-red-600 dark:text-rose-400"">{errorMessage}</p>");
        }

        output.Content.SetHtmlContent(sb.ToString());
    }
}
