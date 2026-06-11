namespace Cadlix_backend.Api;

public class UploadContentMediaDto
{
    public IFormFile? VideoFile { get; set; }
    public IFormFile? PosterFile { get; set; }
    public IFormFile? ThumbnailFile { get; set; }
    public IFormFile? BackdropFile { get; set; }
}
