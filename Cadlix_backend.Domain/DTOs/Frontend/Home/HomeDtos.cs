namespace Cadlix_backend.Domain.DTOs.Frontend;

public class ContentCardDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public double Score { get; set; }
    public int Year { get; set; }
    public string Poster { get; set; } = string.Empty;
}

public class FeaturedContentDto
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
    public double Score { get; set; }
    public string Views { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
}

public class HomePayloadDto
{
    public FeaturedContentDto Featured { get; set; } = new();
    public List<ContentCardDto> TrendingRow { get; set; } = new();
    public List<ContentCardDto> NewReleases { get; set; } = new();
    public List<ContentCardDto> TopRated { get; set; } = new();
}
