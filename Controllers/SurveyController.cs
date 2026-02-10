using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zad1.Data;
using zad1.Models;
using zad1.Models.ViewModels;

namespace zad1.Controllers;

public class SurveyController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SurveyController> _logger;

    public SurveyController(ApplicationDbContext context, ILogger<SurveyController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var surveys = await _context.Surveys
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
        return View(surveys);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var survey = await _context.Surveys
            .Include(s => s.Options)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (survey == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name;
        var hasVoted = false;
        var canVote = User.Identity?.IsAuthenticated == true;
        var isCreator = User.Identity?.IsAuthenticated == true && 
                       survey.CreatedBy == userName;

        if (userId != null)
        {
            var userVote = await _context.Votes
                .Include(v => v.Option)
                .Where(v => v.UserId == userId && v.Option.SurveyId == id)
                .FirstOrDefaultAsync();
            
            hasVoted = userVote != null;
        }

        var viewModel = new SurveyDetailsViewModel
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            CreatedAt = survey.CreatedAt,
            CreatedBy = survey.CreatedBy,
            Options = survey.Options.ToList(),
            HasVoted = hasVoted,
            CanVote = canVote && !hasVoted,
            IsCreator = isCreator
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Ankieter")]
    public IActionResult Create()
    {
        return View(new SurveyViewModel { OptionTexts = new List<string> { "", "" } });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Ankieter")]
    public async Task<IActionResult> Create(SurveyViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var validOptions = viewModel.OptionTexts
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .ToList();

            if (validOptions.Count < 2)
            {
                ModelState.AddModelError(nameof(viewModel.OptionTexts), "At least 2 options are required.");
                viewModel.OptionTexts = new List<string> { "", "" };
                return View(viewModel);
            }

            var survey = new Survey
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? string.Empty,
                Options = validOptions.Select(text => new Option { Text = text.Trim() }).ToList()
            };

            _context.Add(survey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        if (viewModel.OptionTexts.Count < 2)
        {
            viewModel.OptionTexts = new List<string> { "", "" };
        }

        return View(viewModel);
    }

    [Authorize]
    public async Task<IActionResult> Results(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var survey = await _context.Surveys
            .Include(s => s.Options)
                .ThenInclude(o => o.Votes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (survey == null)
        {
            return NotFound();
        }

        var optionResults = survey.Options.Select(option => new OptionResult
        {
            OptionId = option.Id,
            OptionText = option.Text,
            VoteCount = option.Votes.Count
        }).ToList();

        var viewModel = new SurveyResultsViewModel
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            OptionResults = optionResults
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Ankieter")]
    public async Task<IActionResult> MySurveys()
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return RedirectToAction(nameof(Index));
        }

        var surveys = await _context.Surveys
            .Where(s => s.CreatedBy == userName)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return View(surveys);
    }
}
