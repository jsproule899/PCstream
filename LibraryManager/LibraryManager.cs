using MvcMovie.Data;

namespace LibraryManager;

public class Manager(MvcMovieContext context, LibraryRegistry registry)
{
    private readonly MvcMovieContext _context = context;
    private readonly LibraryRegistry _registry = registry;

    public async Task Scan()
    {
        foreach (var lib in _registry.Libraries)
        {
            await lib.Scan(_context);
        }
    }
}