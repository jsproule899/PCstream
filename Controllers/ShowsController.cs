using MvcMovie.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    [Route("Shows")]
    public class ShowsController(MvcMovieContext context) : Controller
    {

        private readonly MvcMovieContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {

            if (_context.Show == null)
            {
                return Problem("Entity set 'MvcMovieContext.Show' is null.");
            }

            IQueryable<Show> shows = from s in _context.Show orderby s.Title select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                shows = shows.Where(s => s.Title!.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
            }
            return View(await shows.ToListAsync());
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {

            if (_context.Show == null)
            {
                return Problem("Entity set 'MvcMovieContext.Show' is null.");
            }

            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show.Include("Seasons")
                .FirstOrDefaultAsync(s => s.Id == id);


            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        [HttpGet("{id}/Season/{seasonNum}")]
        public async Task<IActionResult> Season(int? id, int? seasonNum)
        {

            if (_context.Show == null)
            {
                return Problem("Entity set 'MvcMovieContext.Show' is null.");
            }

            if (id == null || seasonNum == null)
            {
                return NotFound();
            }

            var season = await _context.Season.Include("Episodes").Include("Episodes.Video")
                .FirstOrDefaultAsync(s => s.ShowId == id && s.SeasonNumber == seasonNum);

            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }
    }
}
