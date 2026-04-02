using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardAction _leaderboardService;

        public LeaderboardController()
            => _leaderboardService = new BusinessLogic().Leaderboard();

        [HttpGet]
        public IActionResult GetTop([FromQuery] int count = 100)
        {
            var result = _leaderboardService.GetTopUsers(count);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        // [Authorize]
        public IActionResult GetMyRank(int userId)
        {
            var result = _leaderboardService.GetUserRank(userId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}
