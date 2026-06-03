using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Services;
using Cadlix_backend.Domain.DTOs;

namespace Cadlix_backend.BusinessLayer.Structure;

public class UserActionExecution : UserActions, IUserAction
{
    public IEnumerable<UserDTO> GetAllUsers()
    {
        return GetAllUsersAsync().GetAwaiter().GetResult();
    }

    public UserDTO GetUserById(int id)
    {
        return GetUserByIdAsync(id).GetAwaiter().GetResult();
    }

    public UserDTO CreateUser(CreateUserDTO createDto)
    {
        return CreateUserAsync(createDto).GetAwaiter().GetResult();
    }

    public UserDTO UpdateUser(int id, UpdateUserDTO updateDto)
    {
        return UpdateUserAsync(id, updateDto).GetAwaiter().GetResult();
    }

    public void DeleteUser(int id)
    {
        DeleteUserAsync(id).GetAwaiter().GetResult();
    }
}
