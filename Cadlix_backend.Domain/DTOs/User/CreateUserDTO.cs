using Cadlix_backend.Domain.Entities.User;

namespace Cadlix_backend.Domain.DTOs;

public class CreateUserDTO
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
