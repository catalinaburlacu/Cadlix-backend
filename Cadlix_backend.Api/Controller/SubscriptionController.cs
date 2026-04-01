using System.Security.Claims;
using Cadlix_backend.BusinessLogic.Services;
using Cadlix_backend.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetActive()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User id claim is missing or invalid.");
            }

            var sub = await _subscriptionService.GetActiveSubscriptionAsync(userId);
            return Ok(sub);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionDTO dto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User id claim is missing or invalid.");
            }

            dto.UserId = userId;
            var result = await _subscriptionService.CreateSubscriptionAsync(dto);
            return Ok(result);
        }

        [HttpPost("cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized("User id claim is missing or invalid.");
            }

            await _subscriptionService.CancelSubscriptionAsync(userId);
            return Ok("Subscription was cancelled successfully.");
        }

        private bool TryGetUserId(out int userId)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out userId);
        }
    }
}
