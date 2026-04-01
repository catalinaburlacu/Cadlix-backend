using System;

namespace Cadlix_backend.Domain.DTOs.Movie;

public class GetMovieDetailDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public string? Country { get; set; }
    public DateTime ReleaseDate { get; set; }
    public double Rating { get; set; }
    public string? Description { get; set; }
}
