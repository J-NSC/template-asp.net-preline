using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-multiselect", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class UiMultiSelectTagHelper : TagHelper
{
    // Label e identificação
    [HtmlAttributeName("label")] public string? Label { get; set; }
    [HtmlAttributeName("id")] public string? Id { get; set; }
    [HtmlAttributeName("name")] public string? Name { get; set; }

    // Dados
    [HtmlAttributeName("items")] public IEnumerable<SelectListItem>? Items { get; set; }
    [HtmlAttributeName("selected-values")] public IEnumerable<string>? SelectedValues { get; set; }

    // Comportamento
    [HtmlAttributeName("multiple")] public bool Multiple { get; set; } = true;
    [HtmlAttributeName("required")] public bool Required { get; set; } = false;
    [HtmlAttributeName("placeholder")] public string Placeholder { get; set; } = "Selecione...";

    // Estilos/containers
    [HtmlAttributeName("container-class")] public string ContainerClass { get; set; } = "w-full";
    [HtmlAttributeName("class")] public string? ExtraSelectClass { get; set; } // classes extras no <select>

    // HS Select (Preline) – você pode sobrescrever se quiser
    [HtmlAttributeName("toggle-classes")] public string ToggleClasses { get; set; } =
        "hs-select-disabled:pointer-events-none hs-select-disabled:opacity-50 " +
        "relative py-3 ps-4 pe-9 flex gap-x-2 text-nowrap w-full cursor-pointer " +
        "bg-white border border-gray-200 rounded-lg text-start text-sm focus:outline-hidden focus:ring-2 focus:ring-blue-500 " +
        "dark:bg-neutral-900 dark:border-neutral-700 dark:text-neutral-400 dark:focus:outline-hidden dark:focus:ring-1 dark:focus:ring-neutral-600";

    [HtmlAttributeName("dropdown-classes")] public string DropdownClasses { get; set; } =
        "mt-2 z-50 w-full max-h-72 p-1 space-y-0.5 bg-white border border-gray-200 rounded-lg overflow-hidden overflow-y-auto " +
        "[&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-track]:bg-gray-100 [&::-webkit-scrollbar-thumb]:bg-gray-300 " +
        "dark:[&::-webkit-scrollbar-track]:bg-neutral-700 dark:[&::-webkit-scrollbar-thumb]:bg-neutral-500 dark:bg-neutral-900 dark:border-neutral-700";

    [HtmlAttributeName("option-classes")] public string OptionClasses { get; set; } =
        "py-2 px-4 w-full text-sm text-gray-800 cursor-pointer hover:bg-gray-100 rounded-lg " +
        "focus:outline-hidden focus:bg-gray-100 dark:bg-neutral-900 dark:hover:bg-neutral-800 dark:text-neutral-200 dark:focus:bg-neutral-800";

    [HtmlAttributeName("option-template")] public string OptionTemplate { get; set; } =
        "<div class=\\\"flex justify-between items-center w-full\\\">" +
        "<span data-title></span>" +
        "<span class=\\\"hidden hs-selected:block\\\">" +
        "<svg class=\\\"shrink-0 size-3.5 text-blue-600 dark:text-blue-500\\\" xmlns=\\\"http://www.w3.org/2000/svg\\\" width=\\\"24\\\" height=\\\"24\\\" viewBox=\\\"0 0 24 24\\\" fill=\\\"none\\\" stroke=\\\"currentColor\\\" stroke-width=\\\"2\\\" stroke-linecap=\\\"round\\\" stroke-linejoin=\\\"round\\\"><polyline points=\\\"20 6 9 17 4 12\\\"/></svg>" +
        "</span></div>";

    [HtmlAttributeName("extra-markup")] public string ExtraMarkup { get; set; } =
        "<div class=\\\"absolute top-1/2 end-3 -translate-y-1/2\\\">" +
        "<svg class=\\\"shrink-0 size-3.5 text-gray-500 dark:text-neutral-500\\\" xmlns=\\\"http://www.w3.org/2000/svg\\\" width=\\\"24\\\" height=\\\"24\\\" viewBox=\\\"0 0 24 24\\\" fill=\\\"none\\\" stroke=\\\"currentColor\\\" stroke-width=\\\"2\\\" stroke-linecap=\\\"round\\\" stroke-linejoin=\\\"round\\\"><path d=\\\"m7 15 5 5 5-5\\\"/><path d=\\\"m7 9 5-5 5 5\\\"/></svg>" +
        "</div>";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", ContainerClass);

        var id = Id ?? Name ?? $"ms_{Guid.NewGuid():N}";
        var name = Name ?? id;

        // JSON de config do hs-select
        // (escape de aspas já aplicado nas props default)
        var hsJson = new StringBuilder()
            .Append("{")
            .Append($"\"placeholder\": \"{Escape(Placeholder)}\",")
            .Append("\"toggleTag\": \"<button type=\\\"button\\\" aria-expanded=\\\"false\\\"></button>\",")
            .Append($"\"toggleClasses\": \"{ToggleClasses}\",")
            .Append($"\"dropdownClasses\": \"{DropdownClasses}\",")
            .Append($"\"optionClasses\": \"{OptionClasses}\",")
            .Append($"\"optionTemplate\": \"{OptionTemplate}\",")
            .Append($"\"extraMarkup\": \"{ExtraMarkup}\"")
            .Append("}")
            .ToString();

        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(Label))
        {
            sb.AppendLine($"<label for=\"{id}\" class=\"block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300\">{Label}</label>");
        }

        // select original fica hidden — o HS Select transforma
        var selectClasses = "hidden";
        if (!string.IsNullOrWhiteSpace(ExtraSelectClass)) selectClasses += " " + ExtraSelectClass;

        sb.Append($"<select {(Multiple ? "multiple" : "")} id=\"{id}\" name=\"{name}\" " +
                  $"data-hs-select='{hsJson}' class=\"{selectClasses}\" {(Required ? "required" : "")}>");

        // placeholder como primeira option vazia (não selecionável em múltiplo, mas mantemos por fallback)
        sb.Append($"<option value=\"\">{Escape(Placeholder)}</option>");

        if (Items != null)
        {
            var selectedSet = new HashSet<string>(SelectedValues ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            foreach (var it in Items)
            {
                var val = it.Value ?? it.Text;
                var isSelected = it.Selected || selectedSet.Contains(val ?? string.Empty);
                var sel = isSelected ? " selected" : string.Empty;
                sb.Append($"<option value=\"{Html(val)}\"{sel}>{Html(it.Text)}</option>");
            }
        }

        sb.Append("</select>");

        output.Content.SetHtmlContent(sb.ToString());
    }

    private static string Escape(string? s) =>
        (s ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");

    private static string Html(string? s) =>
        System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
}
