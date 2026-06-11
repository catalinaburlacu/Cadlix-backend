using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Utilities;
using Cadlix_backend.DataAccess.Context;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.DTOs.User;
using Cadlix_backend.Domain.Entities.User;
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
            var refreshToken = jwt.GenerateRefreshToken();

            StoreRefreshToken(user.Id, refreshToken, jwt.GetRefreshTokenExpiresAt());
            SetRefreshTokenCookie(refreshToken, jwt.GetRefreshTokenExpiresAt());

            return Ok(new AuthResponseDto
            {
                User = user,
                Token = token,
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
            var refreshToken = jwt.GenerateRefreshToken();

            StoreRefreshToken(created.Id, refreshToken, jwt.GetRefreshTokenExpiresAt());
            SetRefreshTokenCookie(refreshToken, jwt.GetRefreshTokenExpiresAt());

            return Ok(new AuthResponseDto
            {
                User = created,
                Token = token,
                ExpiresAt = jwt.GetAccessTokenExpiresAt(),
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Refresh token is required.");

            using var db = new AppDbContext();
            var storedToken = db.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken);

            if (storedToken == null)
                return Unauthorized("Invalid or expired refresh token.");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
            {
                db.RefreshTokens.Remove(storedToken);
                db.SaveChanges();
                return Unauthorized("Refresh token has expired.");
            }

            var user = _userService.GetUserById(storedToken.UserId);
            if (user == null)
                return Unauthorized("User no longer exists.");

            var jwt = new JWT();
            var newToken = jwt.GenerateJWTToken(user);
            var newRefreshToken = jwt.GenerateRefreshToken();

            db.RefreshTokens.Remove(storedToken);
            db.RefreshTokens.Add(new RefreshTokenData
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = jwt.GetRefreshTokenExpiresAt(),
                CreatedAt = DateTime.UtcNow,
            });
            db.SaveChanges();

            SetRefreshTokenCookie(newRefreshToken, jwt.GetRefreshTokenExpiresAt());

            return Ok(new AuthResponseDto
            {
                User = user,
                Token = newToken,
                ExpiresAt = jwt.GetAccessTokenExpiresAt(),
            });
        }

        private void StoreRefreshToken(int userId, string token, DateTime expiresAt)
        {
            using var db = new AppDbContext();
            db.RefreshTokens.Add(new RefreshTokenData
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
            });
            db.SaveChanges();
        }

        private void SetRefreshTokenCookie(string token, DateTime expiresAt)
        {
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                Path = "/api/Login/refresh",
            });
        }
    }
}
