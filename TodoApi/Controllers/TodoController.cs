using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Authorize]
[ApiController]
[Route("todos")]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _context;

    public TodoController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDto request)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null) return NotFound();

        if (todo.UserId != GetUserId()) return StatusCode(404, new { message = "Forbidden" });

        todo.Title = request.Title;
        todo.Description = request.Description;

        await _context.SaveChangesAsync();
        return Ok(new TodoResponseDto(todo.Id, todo.Title, todo.Description));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null) return NotFound();
        if (todo.UserId != GetUserId()) return StatusCode(403, new { message = "Forbidden" });

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var userId = GetUserId();
        var query = _context.Todos.Where(t => t.UserId == userId);

        var total = await query.CountAsync();
        var todos = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(t => new TodoResponseDto(t.Id, t.Title, t.Description))
            .ToListAsync();

        return Ok(new PaginatedReponse<TodoResponseDto>(todos, page, limit, total));
    }
}
