using Cadlix_backend.BusinessLayer;
using Cadlix_backend.BusinessLayer.Interfaces;
using Cadlix_backend.Domain.DTOs.Lists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadlix_backend.Api.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ListsController : ControllerBase
{
    private readonly IListsAction _listsService;

    public ListsController()
    {
        _listsService = new BusinessLogic().Lists();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var lists = _listsService.GetAllLists();
        return Ok(lists);
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetByUserId(int userId)
    {
        if (!IsAuthorizedForUser(userId))
            return Forbid();

        var lists = _listsService.GetListsByUserId(userId);
        return Ok(lists);
    }

    [HttpGet("user/{userId}/status/{status}")]
    public IActionResult GetByStatus(int userId, string status)
    {
        if (!IsAuthorizedForUser(userId))
            return Forbid();

        var lists = _listsService.GetListsByStatus(userId, status);
        return Ok(lists);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var list = _listsService.GetListById(id);
        if (list == null)
            return NotFound();

        return Ok(list);
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateListDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _listsService.CreateList(dto);
        return Ok(created);
    }

    [HttpPost("user/{userId}/create")]
    public IActionResult CreateForUser(int userId, [FromBody] CreateListDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!IsAuthorizedForUser(userId))
            return Forbid();

        dto.UserId = userId;
        var created = _listsService.CreateList(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateListDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = _listsService.UpdateList(id, dto);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpPut("{id}/status/{status}")]
    public IActionResult UpdateStatus(int id, string status)
    {
        var ok = _listsService.UpdateFilmStatus(id, status);
        if (!ok)
            return NotFound();

        return Ok(new { message = "Status updated successfully." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var list = _listsService.GetListById(id);
        if (list == null)
            return NotFound();

        if (!IsAuthorizedForUser(list.UserId))
            return Forbid();

        var ok = _listsService.DeleteList(id);
        if (!ok)
            return NotFound();

        return Ok(new { message = "List entry deleted successfully." });
    }

    [HttpDelete("user/{userId}")]
    public IActionResult DeleteUserLists(int userId)
    {
        if (!IsAuthorizedForUser(userId))
            return Forbid();

        _listsService.DeleteUserLists(userId);
        return Ok(new { message = "All user lists deleted successfully." });
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] int? userId, [FromQuery] string? query)
    {
        var lists = _listsService.GetAllLists();

        if (userId.HasValue)
        {
            if (!IsAuthorizedForUser(userId.Value))
                return Forbid();

            lists = lists.Where(l => l.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            lists = lists.Where(l =>
                (l.FilmTitle?.ToLower().Contains(lowerQuery) ?? false) ||
                (l.Type?.ToLower().Contains(lowerQuery) ?? false) ||
                (l.Category?.ToLower().Contains(lowerQuery) ?? false) ||
                (l.FilmStatus?.ToLower().Contains(lowerQuery) ?? false));
        }

        return Ok(lists);
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
