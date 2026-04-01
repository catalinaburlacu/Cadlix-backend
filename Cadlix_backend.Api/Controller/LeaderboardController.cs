using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cadlix_backend.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Cadlix_backend.DataAccess.Repositories.Interfaces;
using System.Security.Claims;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
            private readonly ILeaderboardRepository _leaderboardService;

    public LeaderboardController(ILeaderboardRepository leaderboardService)
        => _leaderboardService = leaderboardService;

    [HttpGet]
    public async Task<IActionResult> GetTop([FromQuery] int count = 100)
    {
        var result = await _leaderboardService.GetTopUsersAsync(count);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyRank()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _leaderboardService.GetUserRankAsync(userId);
        return Ok(result);
    }

    }
}
