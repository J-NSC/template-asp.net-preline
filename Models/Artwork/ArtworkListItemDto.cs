namespace api_doc.Models.Artwork;

public class ArtworkListItemDto
{
    public string Id { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? CreationDate { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Dimensions { get; set; }
    public int? Stock { get; set; }
    public string? Material { get; set; }
    public string? ArtType { get; set; }
    public string? Object3DPath { get; set; }
    public string? IosObject3DPath { get; set; }
    public string? ImageObjectPath { get; set; }
    public string? OrientationType { get; set; }
    public List<TechniqueReponseDto>? Techniques { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}
