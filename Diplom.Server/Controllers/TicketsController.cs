using Diplom.Server.Data;
using Diplom.Server.Services;
using Diplom.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TicketsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Отримання списку тікетів (Read all)
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var tickets = await _context.Tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        return Ok(tickets);
    }

    // 2. Отримання тікету за ID (Read one)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    // 3. Створення нового тікету (Create)
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return Ok(ticket);
    }

    // 4. Оновлення тікету (Update)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
    {
        if (id != ticket.Id)
            return BadRequest("Id in URL doesn't match Ticket.Id");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingTicket = await _context.Tickets.FindAsync(id);
        if (existingTicket == null)
            return NotFound();

        // Оновлюємо поля
        existingTicket.Title = ticket.Title;
        existingTicket.Description = ticket.Description;
        existingTicket.Category = ticket.Category;
        existingTicket.Location = ticket.Location;
        existingTicket.TicketType = ticket.TicketType;
        existingTicket.ChatGroupName = ticket.ChatGroupName;
        // CreatedAt не змінюємо, якщо не потрібно

        _context.Tickets.Update(existingTicket);
        await _context.SaveChangesAsync();
        return Ok(existingTicket);
    }

    // 5. Видалення тікету (Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return NotFound();

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return Ok();
    }
}