
using Microsoft.EntityFrameworkCore;

namespace MvcMovie.Data
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
            
        {
        }

        public DbSet<MvcMovie.Models.Movie> Movie { get; set; } = default!;

        public DbSet<MvcMovie.Models.Video> Video { get; set; } = default!;

        public DbSet<MvcMovie.Models.Show> Show { get; set; } = default!;
        public DbSet<MvcMovie.Models.Season> Season { get; set; } = default!;

        public DbSet<MvcMovie.Models.Episode> Episode { get; set; } = default!;
        
    }
}
