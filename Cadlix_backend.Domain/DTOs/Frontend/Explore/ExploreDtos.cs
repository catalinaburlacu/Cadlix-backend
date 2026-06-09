namespace Cadlix_backend.Domain.DTOs.Frontend;

public class ExploreCategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public List<string> Items { get; set; } = new();
}

public class ExploreMovieDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public double? Rating { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
}

public class CarouselItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;
}

public class CarouselRowDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<CarouselItemDto> Items { get; set; } = new();
}

public class ExplorePayloadDto
{
    public List<ExploreCategoryDto> Categories { get; set; } = new();
    public List<ExploreMovieDto> MovieDatabase { get; set; } = new();
    public List<CarouselRowDto> CarouselRows { get; set; } = new();
}
