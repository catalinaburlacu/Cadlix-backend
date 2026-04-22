using System.Security.Claims;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.BusinessLayer;
using Cadlix_backend.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller
{
    [Route("api/[controller]/{userId}")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionAction _subscriptionService;

        public SubscriptionController()
        {
            _subscriptionService = new BusinessLogic().Subscription();
        }

        [HttpGet("active")]
         [Authorize]
        public IActionResult GetActive(int userId)
        {
             if (!TryGetUserId(out var validatedUserId))
             {
                 return Unauthorized("User id claim is missing or invalid.");
             }
            SubscriptionDTO result;
            try
            {
                result = _subscriptionService.GetActiveSubscription(validatedUserId);
            }
            catch
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("create")]
         [Authorize]
        public IActionResult Create(int userId, [FromBody] CreateSubscriptionDTO dto)
        {
             if (!TryGetUserId(out var validatedUserId))
             {
                 return Unauthorized("User id claim is missing or invalid.");
             }

            SubscriptionDTO result;
            try
            {
                dto.UserId = validatedUserId;
                result = _subscriptionService.CreateSubscription(dto);
            }
            catch
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("cancel")]
         [Authorize]
        public IActionResult Cancel(int userId)
        {
             if (!TryGetUserId(out var validatedUserId))
             {
                 return Unauthorized("User id claim is missing or invalid.");
             }
            
            try
            {
                _subscriptionService.CancelSubscription(validatedUserId);
            }
            catch
            {
                return NotFound();
            }
            return Ok("Subscription was cancelled successfully.");
        }

         private bool TryGetUserId(out int userId)
        {
             userId = 1;
             return true;
         }
    }
}
