using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zad1.Models;

public class Option
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Text { get; set; } = string.Empty;

    public int SurveyId { get; set; }

    [ForeignKey(nameof(SurveyId))]
    public Survey Survey { get; set; } = null!;

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
