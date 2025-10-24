using Microsoft.AspNetCore.Mvc;

namespace api_doc.Models.Ui;

public static class Breadcrumbs
{
    public static BreadcrumbItem Tail(Controller controller, string label, string? url = null, string? iconPath = null)
    {
        var tail = new BreadcrumbItem(label, url, iconPath);
        if (tail.Parent is null)
            tail.From(BreadcrumbItem.Home(controller.Url));
        return tail;
    }

    public static BreadcrumbItem Parent(this BreadcrumbItem child, Controller controller, string label, string? url = null, string? iconPath = null)
        => child.From(new BreadcrumbItem(label, url, iconPath));
}