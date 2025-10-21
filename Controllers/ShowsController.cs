using MvcMovie.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace MvcMovie.Controllers
{

    public class ShowsController : Controller
    {

         private readonly MvcMovieContext _context;

        public ShowsController(MvcMovieContext context)
        {
            _context = context;
        }


        //GET Shows
        public async Task<IActionResult> Index(string searchString)
        {

            if (_context.Show == null)
            {
                return Problem("Entity set 'MvcMovieContext.Show' is null.");
            }

            var shows = from s in _context.Show select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                shows = shows.Where(s => s.Title!.ToLower().Contains(searchString.ToLower()));
            }
            return View(await shows.ToListAsync());
        }

        

        // GET: Show/5
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

        //GET: Show/5/Season/1
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
