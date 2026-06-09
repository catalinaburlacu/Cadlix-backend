using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer.Utilities;
using Cadlix_backend.Domain.DTOs;
using Cadlix_backend.Domain.DTOs.Frontend;
using Cadlix_backend.Domain.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserAction _userService;

        public UserController()
        {
            _userService = new BusinessLogic().User();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserDTO dto)
        {
            try
            {
                var created = _userService.CreateUser(dto);
                if (created == null)
                    return BadRequest(new { message = "Email already registered." });

                var token = new JWT().GenerateJWTToken(created);
                return Ok(new AuthResponseDto
                {
                    User = created,
                    Token = token
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already registered"))
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserDTO dto)
        {
            var updated = _userService.UpdateUser(id, dto);
            if (updated is null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _userService.GetUserById(id);
            if (existing == null)
                return NotFound();

            _userService.DeleteUser(id);
            return Ok(new MessageResponseDto
            {
                Message = "User was deleted successfully."
            });
        }

    }
}