using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-modal-footer", ParentTag = "ui-modal")]
public sealed class UiModalFooterTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var modalCtx = (ModalContext)context.Items[typeof(ModalContext)]!;
        modalCtx.FooterHtml = (await output.GetChildContentAsync()).GetContent();
        output.SuppressOutput();
    }
}
