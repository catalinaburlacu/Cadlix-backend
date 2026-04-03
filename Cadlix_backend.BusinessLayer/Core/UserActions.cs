    using Cadlix_backend.BusinessLayer.Exceptions;
    using Cadlix_backend.DataAccess.Context;
    using Cadlix_backend.DataAccess.Repositories;
    using Cadlix_backend.DataAccess.Repositories.Interfaces;
    using Cadlix_backend.Domain.DTOs;
    using Cadlix_backend.Domain.Entities.User;

    namespace Cadlix_backend.BusinessLayer.Services;

    public class UserActions
    {
        private readonly IUserRepository _repo;

        public UserActions()
        {
            _repo = new UserRepository(new AppDbContext());
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("User was not found.");

            return MapToDto(user);
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createDto)
        {
            var entity = new UserData
            {
                Name = createDto.Name,
                Password = createDto.Password,
                Email = createDto.Email,
                Level = createDto.Level,
                HistoryId = createDto.HistoryId,
                MovieListId = createDto.MovieListId
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateDto)
        {
            var existing = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("User was not found.");

            existing.Name = updateDto.Name;
            existing.Email = updateDto.Email;
            existing.Level = updateDto.Level;
            existing.HistoryId = updateDto.HistoryId;
            existing.MovieListId = updateDto.MovieListId;

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                existing.Password = updateDto.Password;
            }

            var updated = await _repo.UpdateAsync(existing)
                ?? throw new NotFoundException("User was not found.");

            return MapToDto(updated);
        }

        public async Task DeleteUserAsync(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
            {
                throw new NotFoundException("User was not found.");
            }
        }

        private static UserDTO MapToDto(UserData user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Level = user.Level,
                HistoryId = user.HistoryId,
                MovieListId = user.MovieListId
            };
        }
    }
