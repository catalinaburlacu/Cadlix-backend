using System;
using System.ComponentModel.DataAnnotations;

namespace Cadlix_backend.Domain.DTOs.Movie;

public class UploadMovieDTO
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
    public string? Title { get; set; }

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
    public double? Rating { get; set; }

    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; set; }

    public int? Year { get; set; }

    [StringLength(100, ErrorMessage = "Director cannot exceed 100 characters")]
    public string? Director { get; set; }

    public List<string>? Genres { get; set; }

    public List<string>? Country { get; set; }

    public List<string>? Cast { get; set; }

    [StringLength(50, ErrorMessage = "Duration format error")]
    public string? Duration { get; set; }

    public int? DurationSeconds { get; set; }

    // For UI feedback
    public string? FileName { get; set; }

    public long? FileSize { get; set; }
}

public class MovieUploadResponseDTO
{
    public int MovieId { get; set; }
    public string? Title { get; set; }
    public string? VideoFileName { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
}

public class VideoInfoDTO
{
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? StreamUrl { get; set; }
}
