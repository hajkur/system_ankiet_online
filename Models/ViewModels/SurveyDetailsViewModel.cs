using zad1.Models;

namespace zad1.Models.ViewModels;

public class SurveyDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<Option> Options { get; set; } = new();
    public bool HasVoted { get; set; }
    public bool CanVote { get; set; }
    public bool IsCreator { get; set; }
}
