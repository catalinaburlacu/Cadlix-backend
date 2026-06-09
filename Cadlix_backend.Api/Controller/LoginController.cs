using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Utilities;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserAction _userService;

        public LoginController()
        {
            _userService = new BusinessLogic().User();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = _userService.Login(dto);
            if (user is null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var jwt = new JWT();
            var token = jwt.GenerateJWTToken(user);
            var refreshToken = jwt.GenerateRefreshToken(user.Id);

            return Ok(new AuthResponseDto
            {
                User = user,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = jwt.GetAccessTokenExpiresAt(),
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] CreateUserDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Name, email, and password are required.");
            }

            var created = _userService.CreateUser(dto);
            if (created is null)
            {
                return BadRequest("Email already exists.");
            }

            var jwt = new JWT();
            var token = jwt.GenerateJWTToken(created);
            var refreshToken = jwt.GenerateRefreshToken(created.Id);

            return Ok(new AuthResponseDto
            {
                User = created,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = jwt.GetAccessTokenExpiresAt(),
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest("Refresh token is required.");

            var jwt = new JWT();
            var userId = jwt.ValidateRefreshToken(dto.RefreshToken);
            if (userId == null)
                return Unauthorized("Invalid or expired refresh token.");

            var user = _userService.GetUserById(userId.Value);
            if (user == null)
                return Unauthorized("User no longer exists.");

            var newToken = jwt.GenerateJWTToken(user);
            var newRefreshToken = jwt.GenerateRefreshToken(user.Id);

            return Ok(new AuthResponseDto
            {
                User = user,
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = jwt.GetAccessTokenExpiresAt(),
            });
        }
    }
}
