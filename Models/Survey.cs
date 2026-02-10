using System.ComponentModel.DataAnnotations;

namespace zad1.Models;

public class Survey
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string CreatedBy { get; set; } = string.Empty;

    public ICollection<Option> Options { get; set; } = new List<Option>();
}
