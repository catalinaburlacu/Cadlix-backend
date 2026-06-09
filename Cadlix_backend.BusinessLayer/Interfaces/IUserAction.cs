using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.User;

namespace Cadlix_backend.BusinessLayer.Interfaces;

public interface IUserAction
{
    IEnumerable<UserDTO> GetAllUsers();
    UserDTO? GetUserById(int id);
    UserDTO? CreateUser(CreateUserDTO createDto);
    UserDTO? UpdateUser(int id, UpdateUserDTO updateDto);
    bool DeleteUser(int id);
    UserDTO? Login(LoginDTO loginDto);
}
