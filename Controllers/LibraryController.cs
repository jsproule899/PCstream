using LibraryManager;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Data;

namespace MvcMovie.Controllers;

public class LibraryController(Manager manager) : Controller
{
    private readonly Manager _manager = manager;

    public async Task<IActionResult> Rescan()
    {
        await _manager.Scan();
        return Redirect("/");
    }
}