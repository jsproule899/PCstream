using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using MvcMovie.Data;
using LibraryManager;
using System.Composition;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MvcMovieContext") ?? throw new InvalidOperationException("Connection string 'MvcMovieContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

Manager.Initialise(app);
Manager.Add("D:\\Videos");
Manager.Scan();


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
            string path = r.File.PhysicalPath;
            if (path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".svg"))
            {
                TimeSpan maxAge = new TimeSpan(7, 0, 0, 0);
                r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
            }
        }
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.MapControllerRoute(
name: "Library",
pattern: "{controller=Library}/{action=Rescan}");

app.MapControllerRoute(
name: "Shows",
pattern: "{controller=Shows}/{id?}/{action}/{seasonNum?}");

app.MapControllerRoute(
name: "Videos",
pattern: "{controller=Movies}/{action=Player}/{id?}");

app.MapControllerRoute(
name: "Videos",
pattern: "{controller=Movies}/{action=Subtitles}/{id?}/{lang?}");

app.Run();
