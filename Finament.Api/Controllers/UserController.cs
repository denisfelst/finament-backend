using Finament.Application.DTOs.Users.Requests;
using Finament.Application.Mapping;
using Finament.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finament.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly FinamentDbContext _db;

    public UserController(FinamentDbContext db)
    {
        _db = db;
    }
    
    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users.ToListAsync();
        var dto = users.Select(UserMapping.ToDto).ToList();
        return Ok(dto);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();
        
        var dto = UserMapping.ToDto(user);

        return Ok(dto);
    }
    
    // POST: api/users
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var user = UserMapping.ToEntity(dto, null);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        // is email duplicated
        if (dto.Email != null)
        {
            var duplicate = await _db.Users
                .AnyAsync(u => u.Email == dto.Email && u.Id != id);

            if (duplicate)
                return Conflict(new { message = "Email already exists." });
        }
        
        UserMapping.UpdateEntity(user, dto);

        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }
    
    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
