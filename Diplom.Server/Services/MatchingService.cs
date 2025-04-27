using Diplom.Client.Server.Data;
using Diplom.Client.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Client.Server.Services;

public class MatchingService
{
    private readonly ApplicationDbContext _context;
    public MatchingService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Спроба знайти тикет з протилежним типом ("Запит" <-> "Пропозиція") із тією ж категорією та локацією,
    // у якого ще не встановлено ChatGroupName
    public async Task<Ticket> TryMatchTicketAsync(Ticket ticket)
    {
        string oppositeType = ticket.Type == "Запит" ? "Пропозиція" : "Запит";

        var match = await _context.Tickets.FirstOrDefaultAsync(t =>
            t.Type == oppositeType &&
            t.Category == ticket.Category &&
            t.Location == ticket.Location &&
            string.IsNullOrEmpty(t.ChatGroupName));

        if (match != null)
        {
            int minId = ticket.Id < match.Id ? ticket.Id : match.Id;
            int maxId = ticket.Id > match.Id ? ticket.Id : match.Id;
            string groupName = $"match_{minId}_{maxId}";
            ticket.ChatGroupName = groupName;
            match.ChatGroupName = groupName;
            await _context.SaveChangesAsync();
        }
        return match;
    }
}