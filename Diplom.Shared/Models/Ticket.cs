using System.ComponentModel.DataAnnotations;

namespace Diplom.Shared.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва є обов'язковою")]
    [StringLength(100, ErrorMessage = "Назва не може бути довшою за 100 символів")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Опис є обов'язковим")]
    [StringLength(500, ErrorMessage = "Опис не може бути довшим за 500 символів")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Категорія є обов'язковою")]
    public string Category { get; set; }

    [Required(ErrorMessage = "Локація є обов'язковою")]
    public string Location { get; set; }

    [Required(ErrorMessage = "Тип тікету є обов'язковим")]
    public string TicketType { get; set; } // "Запит" або "Пропозиція"

    // При успішному матчінгу заповнюється ім'я групи чату
    public string ChatGroupName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}