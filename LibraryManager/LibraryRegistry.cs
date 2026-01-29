
namespace LibraryManager;

public class LibraryRegistry
{
    private readonly List<Library> _libraries = [];
    private readonly object _lock = new();

    public IReadOnlyList<Library> Libraries
    {
        get
        {
            lock (_lock)
                return _libraries.ToList();
        }
    }

    public void Add(Library lib)
    {
        lock (_lock)
            _libraries.Add(lib);
    }
}

