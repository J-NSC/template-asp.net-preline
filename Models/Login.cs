namespace api_doc.Models;

public sealed class Login
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool    RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
