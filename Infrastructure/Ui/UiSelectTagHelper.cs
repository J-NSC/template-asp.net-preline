using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-select", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class UiSelectTagHelper : TagHelper
{
    [HtmlAttributeName("name")] public string? Name { get; set; }
    [HtmlAttributeName("id")] public string? Id { get; set; }
    [HtmlAttributeName("label")] public string? Label { get; set; }

    [HtmlAttributeName("disabled")] public bool Disabled { get; set; } = false;
    [HtmlAttributeName("full-width")] public bool FullWidth { get; set; } = true;

    [HtmlAttributeName("placeholder")] public string? Placeholder { get; set; }

    /// <summary>
    /// Lista de opções do select — exemplo: new[] { "Admin", "User", "Guest" }
    /// </summary>
    [HtmlAttributeName("items")] public IEnumerable<string>? Items { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(Label))
        {
            sb.AppendLine($@"<label for=""{Id ?? Name}"" class=""block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300"">{Label}</label>");
        }

        var widthClass = FullWidth ? "w-full" : string.Empty;

        sb.AppendLine($@"<select id=""{Id ?? Name}"" name=""{Name}""
            class=""py-3 px-4 pe-9 block {widthClass} bg-gray-100 border-transparent rounded-lg text-sm
                   focus:border-blue-500 focus:ring-blue-500 disabled:opacity-50 disabled:pointer-events-none
                   dark:bg-neutral-700 dark:border-transparent dark:text-neutral-400 dark:focus:ring-neutral-600""
            {(Disabled ? "disabled" : string.Empty)}>");

        if (!string.IsNullOrWhiteSpace(Placeholder))
        {
            sb.AppendLine($@"<option selected disabled>{Placeholder}</option>");
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
            // Permite inserir manualmente conteúdo <option> dentro do taghelper
            var childContent = await output.GetChildContentAsync();
            sb.AppendLine(childContent.GetContent());
        }

        sb.AppendLine("</select>");
        output.Content.SetHtmlContent(sb.ToString());
    }
}
