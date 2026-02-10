using System.ComponentModel.DataAnnotations;

namespace zad1.Models.ViewModels;

public class SurveyViewModel
{
    [Required]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    [Display(Name = "Survey Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "At least one option is required.")]
    [MinLength(2, ErrorMessage = "At least 2 options are required.")]
    [Display(Name = "Options")]
    public List<string> OptionTexts { get; set; } = new();
}
