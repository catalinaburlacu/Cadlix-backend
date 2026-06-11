using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs.Frontend;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller;

[Route("api/profile")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IFrontendAction _frontendService;
    private readonly ILikeAction _likeService;

    public ProfileController()
    {
        var logic = new BusinessLogic();
        _frontendService = logic.Frontend();
        _likeService = logic.Like();
    }

    [HttpGet("{userId}")]
    public ActionResult<UserProfileDto> GetProfile(int userId)
    {
        var profile = _frontendService.GetProfile(userId);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    [HttpPost("{userId}/like")]
    public ActionResult<LikeStatusDto> ToggleLike(int userId)
    {
        var likerId = GetCurrentUserId();
        if (likerId == null)
            return Unauthorized();

        return Ok(_likeService.ToggleLike(likerId.Value, userId));
    }

    [HttpGet("{userId}/like")]
    public ActionResult<LikeStatusDto> GetLikeStatus(int userId)
    {
        var likerId = GetCurrentUserId();
        return Ok(_likeService.GetLikeStatus(userId, likerId));
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst("userId")?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(claim, out var id))
            return id;
        return null;
    }
}
