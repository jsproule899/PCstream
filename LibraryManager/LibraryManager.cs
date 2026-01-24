using MvcMovie.Data;

namespace LibraryManager;

class Manager
{
    public static List<Library> libraries = [];

    public static void Add(string path)
    {
        Library lib = new(path);

        libraries.Add(lib);
    }

    public static async Task Scan(MvcMovieContext dbContext)
    {
        foreach (Library lib in libraries)
        {
            lib.Scan(dbContext);
        }
    }

}

