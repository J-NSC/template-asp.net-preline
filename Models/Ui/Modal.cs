namespace api_doc.Models.Ui;

public class Modal
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? BodyHtml { get; set; }
    public string? BodyText { get; set; }
    public bool ShowCloseButton { get; set; } = true;
    public string? PrimaryText { get; set; } = "Salvar";
    public string? SecondaryText { get; set; } = "Fechar";
    public string? PrimaryOnClick { get; set; }
    public bool DarkCapable { get; set; } = true;
}
