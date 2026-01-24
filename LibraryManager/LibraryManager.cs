using System.Threading.Tasks;
using MvcMovie.Data;

namespace LibraryManager;

public class Manager
{
    private readonly MvcMovieContext _context;
    public List<Library> Libraries { get; } = [];
    public Manager(MvcMovieContext context)
    {
        _context = context;
    }

    public void Add(string path)
    {
        Libraries.Add(new Library(path));
    }

    public async Task Scan()
    {
        foreach (var lib in Libraries)
        {
            await lib.Scan(_context);
        }
    }
}