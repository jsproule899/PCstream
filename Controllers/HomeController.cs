using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class HomeController(ILogger<HomeController> logger, MvcMovieContext context) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly MvcMovieContext _context = context;

    public async Task<IActionResult> Index(string? searchString)
    {
        if (_context.Movie == null)
        {
            return Problem("Entity set 'MvcMovieContext.Movie' is null.");
        }

        var movies = from m in _context.Movie select m;

        if (!String.IsNullOrEmpty(searchString))
        {
            movies = movies.Where(s => s.Title!.ToLower().Contains(searchString.ToLower()));
        }
        return View(await movies.Include("Video").ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
