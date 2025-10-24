using api_doc.Models.Ui;
using Microsoft.AspNetCore.Mvc;

namespace api_doc.ViewComponent;

public class BreadcrumbViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    public IViewComponentResult Invoke(BreadcrumbItem? tail = null, bool autoHome = true)
    {
        tail ??= ViewData["Breadcrumb"] as BreadcrumbItem;

        if (tail == null)
        {
            var home = BreadcrumbItem.Home(Url);
            return View(new List<BreadcrumbItem> { home });
        }

        var stack = new List<BreadcrumbItem>();
        var cur = tail;

        while (cur != null)
        {
            stack.Add(cur);
            cur = cur.Parent;
        }

        if (autoHome && stack[^1].Label != "Home")
            stack.Add(BreadcrumbItem.Home(Url));

        stack.Reverse();
        return View(stack);
    }
}
