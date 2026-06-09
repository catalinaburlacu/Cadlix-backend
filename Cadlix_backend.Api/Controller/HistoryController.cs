using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs.History;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IHistoryAction _historyService;

    public HistoryController()
    {
        _historyService = new BusinessLogic().History();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var history = _historyService.GetAllHistory();
        return Ok(history);
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetByUserId(int userId)
    {
        if (!IsAuthorizedForUser(userId))
            return Forbid();

        var history = _historyService.GetHistoryByUserId(userId);
        return Ok(history);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var history = _historyService.GetHistoryById(id);
        if (history == null)
            return NotFound();

        return Ok(history);
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateHistoryDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _historyService.CreateHistory(dto);
        return Ok(created);
    }

    [HttpPost("user/{userId}/create")]
    public IActionResult CreateForUser(int userId, [FromBody] CreateHistoryDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!IsAuthorizedForUser(userId))
            return Forbid();

        dto.UserId = userId;
        var created = _historyService.CreateHistory(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateHistoryDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var updated = _historyService.UpdateHistory(id, dto);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var history = _historyService.GetHistoryById(id);
        if (history == null)
            return NotFound();

        if (!IsAuthorizedForUser(history.UserId))
            return Forbid();

        var ok = _historyService.DeleteHistory(id);
        if (!ok)
            return NotFound();

        return Ok(new { message = "History entry deleted successfully." });
    }

    [HttpDelete("user/{userId}")]
    public IActionResult DeleteUserHistory(int userId)
    {
        if (!IsAuthorizedForUser(userId))
            return Forbid();

        _historyService.DeleteUserHistory(userId);
        return Ok(new { message = "User history deleted successfully." });
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] int? userId, [FromQuery] string? query)
    {
        var history = _historyService.GetAllHistory();

        if (userId.HasValue)
        {
            if (!IsAuthorizedForUser(userId.Value))
                return Forbid();

            history = history.Where(h => h.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            history = history.Where(h =>
                (h.MovieTitle?.ToLower().Contains(lowerQuery) ?? false) ||
                (h.Category?.ToLower().Contains(lowerQuery) ?? false) ||
                (h.Series?.ToLower().Contains(lowerQuery) ?? false));
        }

        return Ok(history);
    }

    private bool IsAuthorizedForUser(int userId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? User.FindFirst("nameid")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return false;

        return int.TryParse(userIdClaim, out var tokenUserId) && tokenUserId == userId;
    }
}
