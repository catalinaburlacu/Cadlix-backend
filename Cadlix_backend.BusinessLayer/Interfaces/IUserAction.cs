using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IUserAction
{
    IEnumerable<UserDTO> GetAllUsers();
    UserDTO GetUserById(int id);
    UserDTO CreateUser(CreateUserDTO createDto);
    UserDTO UpdateUser(int id, UpdateUserDTO updateDto);
    void DeleteUser(int id);
}
