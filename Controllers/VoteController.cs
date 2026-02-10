using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zad1.Data;
using zad1.Models;

namespace zad1.Controllers;

[Authorize]
public class VoteController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VoteController> _logger;

    public VoteController(ApplicationDbContext context, ILogger<VoteController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int surveyId, int optionId)
    {
        if (surveyId == 0 || optionId == 0)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var option = await _context.Options
            .Include(o => o.Survey)
            .FirstOrDefaultAsync(o => o.Id == optionId && o.SurveyId == surveyId);

        if (option == null)
        {
            return NotFound();
        }

        var existingVote = await _context.Votes
            .Include(v => v.Option)
            .Where(v => v.UserId == userId && v.Option.SurveyId == surveyId)
            .FirstOrDefaultAsync();

        if (existingVote != null)
        {
            TempData["ErrorMessage"] = "You have already voted in this survey.";
            return RedirectToAction("Details", "Survey", new { id = surveyId });
        }

        var vote = new Vote
        {
            OptionId = optionId,
            UserId = userId,
            VotedAt = DateTime.UtcNow
        };

        _context.Add(vote);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Your vote has been recorded!";
        return RedirectToAction("Results", "Survey", new { id = surveyId });
    }
    public async Task<IActionResult> HasVoted(int surveyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { hasVoted = false });
        }

        var hasVoted = await _context.Votes
            .Include(v => v.Option)
            .AnyAsync(v => v.UserId == userId && v.Option.SurveyId == surveyId);

        return Json(new { hasVoted });
    }
}
