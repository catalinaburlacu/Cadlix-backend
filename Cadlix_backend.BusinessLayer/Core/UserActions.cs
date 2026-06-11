using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Utilities;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.User;
using Cadlix_backend.Domain.Entities.User;

namespace Cadlix_backend.BusinessLayer.Core;

public class UserActions : IUserAction
{
    private readonly AppDbContext _context;

    public UserActions()
    {
        _context = new AppDbContext();
    }

    public IEnumerable<UserDTO> GetAllUsers()
    {
        return _context.Users.ToList().Select(MapToDto);
    }

    public UserDTO? GetUserById(int id)
    {
        var user = _context.Users.FirstOrDefault(entity => entity.Id == id);
        if (user == null)
        {
            return null;
        }

        return MapToDto(user);
    }

    public UserDTO? CreateUser(CreateUserDTO createDto)
    {
        // Check if email already exists
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == createDto.Email);
        if (existingUser != null)
        {
            return null;
        }

        var entity = new UserData
        {
            Name = createDto.Name,
            Password = PasswordHelper.HashPassword(createDto.Password),
            Email = createDto.Email,
            Level = Domain.Entities.User.URole.User,
        };

        _context.Users.Add(entity);
        _context.SaveChanges();

        return MapToDto(entity);
    }

    public UserDTO? UpdateUser(int id, UpdateUserDTO updateDto)
    {
        var existing = _context.Users.FirstOrDefault(entity => entity.Id == id);
        if (existing == null)
        {
            return null;
        }

        existing.Name = updateDto.Name;
        existing.Email = updateDto.Email;
        existing.Level = updateDto.Level;

        if (!string.IsNullOrWhiteSpace(updateDto.Password))
        {
            existing.Password = PasswordHelper.HashPassword(updateDto.Password);
        }

        _context.SaveChanges();
        return MapToDto(existing);
    }

    public bool DeleteUser(int id)
    {
        var existing = _context.Users.FirstOrDefault(entity => entity.Id == id);

        if (existing == null)
        {
            return false;
        }
        _context.Users.Remove(existing);
        _context.SaveChanges();

        return true;
    }

    public UserDTO? Login(LoginDTO loginDto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);

        if (user == null || !PasswordHelper.VerifyPassword(loginDto.Password, user.Password))
        {
            return null;
        }

        return MapToDto(user);
    }

    private static UserDTO MapToDto(UserData user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Name = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Level = user.Level,
        };
    }
}

