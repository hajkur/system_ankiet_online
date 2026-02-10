using System.Text.Json;

namespace zad1.Models.ViewModels;

public class SurveyResultsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<OptionResult> OptionResults { get; set; } = new();
    
    public string ChartDataJson => JsonSerializer.Serialize(new
    {
        labels = OptionResults.Select(r => r.OptionText).ToArray(),
        data = OptionResults.Select(r => r.VoteCount).ToArray()
    });
}

public class OptionResult
{
    public int OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int VoteCount { get; set; }
}
