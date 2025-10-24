using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace api_doc.Infrastructure.Ui;

public interface IViewDataAccessor
{
    ViewDataDictionary ViewData { get; set; }
}

public sealed class ViewDataAccessor : IViewDataAccessor
{
    public ViewDataDictionary ViewData { get; set; } = default!;
}
