using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zad1.Models;

public class Vote
{
    public int Id { get; set; }

    public int OptionId { get; set; }

    [ForeignKey(nameof(OptionId))]
    public Option Option { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}
