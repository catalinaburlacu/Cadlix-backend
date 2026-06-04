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

    public ProfileController()
    {
        _frontendService = new BusinessLogic().Frontend();
    }

    [HttpGet("{userId}")]
    public ActionResult<UserProfileDto> GetProfile(int userId)
    {
        var profile = _frontendService.GetProfile(userId);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }
}
