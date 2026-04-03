using Cadlix_backend.Domain.Entities.User;

namespace Cadlix_backend.Domain.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public URole Level { get; set; }
    public int HistoryId { get; set; }
    public int MovieListId { get; set; }
}
