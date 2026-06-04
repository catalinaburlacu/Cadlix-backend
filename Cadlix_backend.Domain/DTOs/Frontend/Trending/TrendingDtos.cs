namespace Cadlix_backend.Domain.DTOs.Frontend;

public class TrendingItemDto
{
    public string Id { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
    public double Score { get; set; }
    public string Views { get; set; } = string.Empty;
    public string TrendPct { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Thumb { get; set; } = string.Empty;
}

public class TrendingPayloadDto
{
    public List<UiOptionDto> Periods { get; set; } = new();
    public List<UiOptionDto> Filters { get; set; } = new();
    public List<TrendingItemDto> Data { get; set; } = new();
}
