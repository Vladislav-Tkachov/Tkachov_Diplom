namespace Diplom.Shared.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string User { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}