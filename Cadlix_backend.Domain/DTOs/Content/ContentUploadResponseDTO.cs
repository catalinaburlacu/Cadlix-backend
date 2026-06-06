namespace Cadlix_backend.Domain.DTOs.Content;

public class ContentUploadResponseDTO
{
    public int ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? VideoFileName { get; set; }
    public string? PosterFileName { get; set; }
    public string? ThumbnailFileName { get; set; }
    public string? BackdropFileName { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
