using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace api_doc.Infrastructure.Ui;

public sealed class ViewDataFilter : IActionFilter
{
    private readonly IViewDataAccessor _accessor;

    public ViewDataFilter(IViewDataAccessor accessor)
    {
        _accessor = accessor;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is Controller controller)
        {
            _accessor.ViewData = controller.ViewData;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // nada a fazer depois
    }
}
