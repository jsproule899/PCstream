using LibraryManager;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class LibraryController : Controller
{  
public async Task<IActionResult> Rescan()
    {
       await Manager.Scan();
       return Redirect("/");
    }
}