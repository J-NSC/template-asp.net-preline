using Microsoft.AspNetCore.Razor.TagHelpers;

namespace api_doc.Infrastructure.Ui;

[HtmlTargetElement("ui-modal-title", ParentTag = "ui-modal")]
public sealed class UiModalTitleTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var modalCtx = (ModalContext)context.Items[typeof(ModalContext)]!;
        modalCtx.TitleHtml = (await output.GetChildContentAsync()).GetContent();
        output.SuppressOutput(); // não renderiza a tag
    }
}
