using LibraryManager;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Data;

namespace MvcMovie.Controllers;

public class LibraryController(MvcMovieContext DbContext) : Controller
{
    public async Task<IActionResult> Rescan()
    {
        await Manager.Scan(DbContext);
        return Redirect("/");
    }
}