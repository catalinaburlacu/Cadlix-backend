using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.Domain.DTOs.Frontend;

public class AuthResponseDto
{
    public UserDTO User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
