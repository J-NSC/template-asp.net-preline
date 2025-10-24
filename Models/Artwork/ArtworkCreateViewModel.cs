namespace api_doc.Models.Artwork;

public class ArtworkCreateViewModel
{
    public List<MaterialReponseDto> Materials { get; set; } = new();
    public List<TechniqueReponseDto> Techniques { get; set; } = new();
    public List<ArtTypeReponseDto> ArtTypes { get; set; } = new();
    public List<ArtStatusReponseDto> Statuses { get; set; } = new();
    public List<OrientationTypeReponseDto> OrientationTypes { get; set; } = new();
    public List<ResourceTypeResponseDto> ResourceType { get; set; } = new();
    public List<LanguageResponseDto> Languages { get; set; }= new();
}
