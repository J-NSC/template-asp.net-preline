namespace api_doc.Models;

public class AccessibilitiesReponseDto
{
    public int Id { get; set; }
    public string? ResourceUrl { get; set; }
    public DateTime CreationDate { get; set; }
    public required string LanguageCode { get; set; }
    public required string LanguageName { get; set; }
    public required string ResourceTypeName { get; set; }
}
