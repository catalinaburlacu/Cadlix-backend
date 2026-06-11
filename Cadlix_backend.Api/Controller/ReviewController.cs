using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller;

[Route("api/review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewAction _reviewService;

    public ReviewController()
    {
        _reviewService = new BusinessLogic().Review();
    }

    [HttpGet("movie/{movieId}")]
    public ActionResult<List<ReviewDTO>> GetByMovie(int movieId)
    {
        var userId = GetCurrentUserId();
        return Ok(_reviewService.GetByMovie(movieId, userId));
    }

    [HttpGet("user/{userId}")]
    public ActionResult<List<ReviewDTO>> GetByUser(int userId)
    {
        var currentUserId = GetCurrentUserId();
        return Ok(_reviewService.GetByUser(userId, currentUserId));
    }

    [HttpGet("{id}")]
    public ActionResult<ReviewDTO> GetById(int id)
    {
        var review = _reviewService.GetById(id);
        if (review == null)
            return NotFound();
        return Ok(review);
    }

    [HttpPost]
    [Authorize]
    public ActionResult<ReviewDTO> Create([FromBody] CreateReviewDTO dto)
    {
        var userId = GetUserIdOrReject();
        if (!userId.HasValue) return Unauthorized();

        var created = _reviewService.Create(userId.Value, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize]
    public ActionResult<ReviewDTO> Update(int id, [FromBody] CreateReviewDTO dto)
    {
        var userId = GetUserIdOrReject();
        if (!userId.HasValue) return Unauthorized();

        var updated = _reviewService.Update(id, userId.Value, dto);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var userId = GetUserIdOrReject();
        if (!userId.HasValue) return Unauthorized();

        var ok = _reviewService.Delete(id, userId.Value);
        if (!ok)
            return NotFound();
        return Ok(new { message = "Review deleted successfully." });
    }

    [HttpPost("{reviewId}/like")]
    [Authorize]
    public IActionResult ToggleLike(int reviewId)
    {
        var userId = GetUserIdOrReject();
        if (!userId.HasValue) return Unauthorized();

        var ok = _reviewService.ToggleLike(reviewId, userId.Value);
        if (!ok)
            return NotFound();
        return Ok(new { message = "Like toggled successfully." });
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst("userId") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out var id))
            return id;
        return null;
    }

    private int? GetUserIdOrReject()
    {
        return GetCurrentUserId();
    }
}
