using System;

namespace Cadlix_backend.Domain.DTOs.Movie;

public class CreateMovieDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public List<string>? Genres { get; set; }
    public List<string>? Country { get; set; }
    public int? Year { get; set; }
    public double Rating { get; set; }
    public string? Description { get; set; }
    public string? VideoSource { get; set; }

}
