using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs;
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
            try
            {
                var user = _userService.GetUserById(id);
                return Ok(user);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserDTO dto)
        {
            var created = _userService.CreateUser(dto);
            var token = new Utils().GenerateJWTToken(created);
            return Ok(new { User = created, Token = token });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                var updated = _userService.UpdateUser(id, dto);
                return Ok(updated);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return Ok("User was deleted successfully.");
            }
            catch
            {
                return NotFound();
            }
        }

    }
}
