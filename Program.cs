using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using LibraryManager;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
string libraryDirectory = builder.Configuration["LibraryDirectory"] ?? "D:\\Videos";
builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MvcMovieContext") ?? throw new InvalidOperationException("Connection string 'MvcMovieContext' not found."))
    );

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using var db = new MvcMovieContext(app.Services.GetRequiredService<DbContextOptions<MvcMovieContext>>());
db.Database.Migrate();

Manager.Add(libraryDirectory);
await Manager.Scan(db);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse =
        r =>
        {
            string path = r.File.PhysicalPath ?? "";
            if (path.EndsWith(".css") || path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".svg"))
            {
                TimeSpan maxAge = new(7, 0, 0, 0);
                r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
            }
            else if (path.EndsWith(".js"))
            {
                TimeSpan maxAge = new(1, 0, 0, 0);
                r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
            }
        }
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Movies",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Library",
    pattern: "{controller=Library}/{action=Rescan}");

app.MapControllerRoute(
    name: "Shows",
    pattern: "{controller=Shows}/{id?}/{action}/{seasonNum?}");

app.MapControllerRoute(
    name: "Videos",
    pattern: "{controller=Video}/{action=Player}/{id?}");

app.MapControllerRoute(
    name: "Videos",
    pattern: "{controller=Video}/{action=Subtitles}/{id?}/{lang?}");

app.Run();
