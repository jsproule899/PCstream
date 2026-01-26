namespace MvcMovie.Models;

public class RecentlyWatched
{
    public int Id { get; set; }
    public Episode? Episode { get; set; }
    public Movie? Movie{ get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.Now;

}