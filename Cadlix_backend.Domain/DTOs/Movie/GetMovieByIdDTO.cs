using System;

namespace Cadlix_backend.Domain.DTOs.Movie;

public class GetMovieByIdDTO
{
    public string? Title { get; set; }

    public string? Link { get; set; } = string.Empty;
}
