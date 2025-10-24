using Microsoft.AspNetCore.Mvc;

namespace api_doc.Models.Ui;


public sealed class BreadcrumbItem
{
    public string Label { get; }
    public string? IconPath { get; }
    public string? Url { get; }
    public BreadcrumbItem? Parent { get; private set; }

    public BreadcrumbItem(string label, string? url = null, string? iconPath = null)
    {
        Label = label;
        Url = url;
        IconPath = iconPath;
    }

    // Encadeia um pai e retorna o nó atual (para fluência)
    public BreadcrumbItem From(BreadcrumbItem parent)
    {
        Parent = parent;
        return this;
    }

    // Fábrica do "Home"
    public static BreadcrumbItem Home(IUrlHelper url)
        => new("Home", url.Action("Index", "Home"),
            /* opcional: */ "m3 9 9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z|9 22 9 12 15 12 15 22");
}
