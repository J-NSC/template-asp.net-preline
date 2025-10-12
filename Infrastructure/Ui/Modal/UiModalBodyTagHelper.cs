using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-modal-body", ParentTag = "ui-modal")]
public sealed class UiModalBodyTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var modalCtx = (ModalContext)context.Items[typeof(ModalContext)]!;
        modalCtx.BodyHtml = (await output.GetChildContentAsync()).GetContent();
        output.SuppressOutput();
    }
}
