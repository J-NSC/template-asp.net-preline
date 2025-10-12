using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-modal", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class UiModalTagHelper : TagHelper
{
    [HtmlAttributeName("id")] public string Id { get; set; } = "hs-basic-modal";
    [HtmlAttributeName("show")] public bool Show { get; set; } = false;
    [HtmlAttributeName("focusable")] public bool Focusable { get; set; } = false;

    // opcionais (se não usar slots)
    [HtmlAttributeName("title")] public string? Title { get; set; }
    [HtmlAttributeName("body-text")] public string? BodyText { get; set; }
    [HtmlAttributeName("body-html")] public string? BodyHtml { get; set; }

    // footer padrão (se não usar slot <ui-modal-footer>)
    [HtmlAttributeName("primary-text")] public string PrimaryText { get; set; } = "Save changes";
    [HtmlAttributeName("primary-onclick")] public string? PrimaryOnclick { get; set; }
    [HtmlAttributeName("secondary-text")] public string SecondaryText { get; set; } = "Close";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        var modalCtx = new ModalContext();
        context.Items[typeof(ModalContext)] = modalCtx;

        // processar filhos (slots)
        _ = await output.GetChildContentAsync();

        var labelId = $"{Id}-label";
        var tabindex = Focusable ? "0" : "-1";

        // usar título/corpo vindos por atributo se não houve slot
        var titleHtml = !string.IsNullOrWhiteSpace(modalCtx.TitleHtml) ? modalCtx.TitleHtml : Title ?? "";
        var bodyHtml = !string.IsNullOrWhiteSpace(modalCtx.BodyHtml)
            ? modalCtx.BodyHtml
            : !string.IsNullOrWhiteSpace(BodyHtml)
                ? BodyHtml
                : !string.IsNullOrWhiteSpace(BodyText)
                    ? $@"<p class=""mt-1 text-gray-800 dark:text-neutral-400"">{BodyText}</p>"
                    : "";

        var sb = new StringBuilder($@"
                  <div id=""{Id}"" class=""hs-overlay hs-overlay-open:opacity-100 hs-overlay-open:duration-500 hidden size-full fixed top-0 start-0 z-80 opacity-0 overflow-x-hidden transition-all overflow-y-auto pointer-events-none"" role=""dialog"" tabindex=""{tabindex}"" aria-labelledby=""{labelId}"">
                    <div class=""sm:max-w-lg sm:w-full m-3 sm:mx-auto"">
                      <div class=""flex flex-col bg-white border border-gray-200 shadow-2xs rounded-xl pointer-events-auto dark:bg-neutral-800 dark:border-neutral-700 dark:shadow-neutral-700/70"">
                        <div class=""flex justify-between items-center py-3 px-4 border-b border-gray-200 dark:border-neutral-700"">
                          <h3 id=""{labelId}"" class=""font-bold text-gray-800 dark:text-white"">
                            {titleHtml}
                          </h3>
                          <button type=""button"" class=""size-8 inline-flex justify-center items-center gap-x-2 rounded-full border border-transparent bg-gray-100 text-gray-800 hover:bg-gray-200 focus:outline-hidden focus:bg-gray-200 disabled:opacity-50 disabled:pointer-events-none dark:bg-neutral-700 dark:hover:bg-neutral-600 dark:text-neutral-400 dark:focus:bg-neutral-600"" aria-label=""Close"" data-hs-overlay=""#{Id}"">
                            <span class=""sr-only"">Close</span>
                            <svg class=""shrink-0 size-4"" xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"">
                              <path d=""M18 6 6 18""></path>
                              <path d=""m6 6 12 12""></path>
                            </svg>
                          </button>
                        </div>
                        <div class=""p-4 overflow-y-auto"">
                          {bodyHtml}
                        </div>");

        // footer: se veio slot, usa; senão renderiza o padrão do seu HTML
        if (!string.IsNullOrWhiteSpace(modalCtx.FooterHtml))
        {
            sb.Append($@"
      <div class=""flex justify-end items-center gap-x-2 py-3 px-4 border-t border-gray-200 dark:border-neutral-700"">
        {modalCtx.FooterHtml}
      </div>");
        }
        else
        {
            var onclick = string.IsNullOrWhiteSpace(PrimaryOnclick) ? "" : $@" onclick=""{PrimaryOnclick}""";
            sb.Append($@"
                    <div class=""flex justify-end items-center gap-x-2 py-3 px-4 border-t border-gray-200 dark:border-neutral-700"">
                      <button type=""button"" class=""py-2 px-3 inline-flex items-center gap-x-2 text-sm font-medium rounded-lg border border-gray-200 bg-white text-gray-800 shadow-2xs hover:bg-gray-50 focus:outline-hidden focus:bg-gray-50 disabled:opacity-50 disabled:pointer-events-none dark:bg-neutral-800 dark:border-neutral-700 dark:text-white dark:hover:bg-neutral-700 dark:focus:bg-neutral-700"" data-hs-overlay=""#{Id}"">
                        {SecondaryText}
                      </button>
                      <button type=""button"" class=""py-2 px-3 inline-flex items-center gap-x-2 text-sm font-medium rounded-lg border border-transparent bg-blue-600 text-white hover:bg-blue-700 focus:outline-hidden focus:bg-blue-700 disabled:opacity-50 disabled:pointer-events-none""{onclick}>
                        {PrimaryText}
                      </button>
                    </div>"); 
        } sb.Append(@"
                  </div>
                </div>
              </div>");

        if (Show)
        {
          sb.Append($@"
              <script>
                (function () {{
console.log({Id})
                  const open = () => (window.HSOverlay && window.HSOverlay.open) ? window.HSOverlay.open('#{Id}')
                                           : document.querySelector('[data-hs-overlay=""#{Id}""]')?.click();
                  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', open);
                  else open();
                }})();
              </script>");
        }

        output.Content.SetHtmlContent(sb.ToString());
    }
}
