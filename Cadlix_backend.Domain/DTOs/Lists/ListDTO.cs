namespace Cadlix_backend.Domain.DTOs.Lists;

public class ListDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FilmId { get; set; }
    public string? ExternalFilmId { get; set; }
    public string FilmTitle { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Genre { get; set; }
    public string? Episode { get; set; }
    public string? Poster { get; set; }
    public DateTime AddedAt { get; set; }
    public string FilmStatus { get; set; } = string.Empty;
    public double FilmRating { get; set; }
    public double? FilmScore { get; set; }
}
