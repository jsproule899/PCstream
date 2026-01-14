using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;

namespace LibraryManager;

class Manager
{
    public static List<Library> libraries = [];
    private static WebApplication _App;

    public static void Add(string path)
    {
        Library lib = new(path);

        libraries.Add(lib);
    }

    public static async Task Scan()
    {
        using (var scope = _App.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            MvcMovieContext context = new(
                                     services.GetRequiredService<
                                         DbContextOptions<MvcMovieContext>>());

            foreach (Library lib in libraries)
            {
                lib.Scan(context);
            }

        }
    }

    public static void Initialise(WebApplication app)
    {
        _App = app;

    }

}

