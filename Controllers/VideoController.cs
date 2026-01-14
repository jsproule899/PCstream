using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using Microsoft.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Mono.TextTemplating;
using MvcMovie.Models;


namespace MvcMovie.Video
{
    public class VideosController : Controller
    {
        private readonly MvcMovieContext _context;

        public VideosController(MvcMovieContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Player(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Video.FirstOrDefaultAsync(v => v.Id == id);
            var episode = await _context.Episode.Include("Season").Where(e => e.Video.Id == id).FirstOrDefaultAsync();
            if (episode == null)
            {
                ViewData["NextEpisode"] = null;
            }
            else
            {
                ViewData["Episode"] = episode;
                var show = await _context.Show.Where(s => s.Id == episode.Season.ShowId).FirstOrDefaultAsync();
                var season = await _context.Season.Where(s => s.Id == episode.SeasonId).FirstOrDefaultAsync();
                ViewData["Show"] = show;
                ViewData["Season"] = season;
                var nextEpisodeCurrentSeason = await _context.Episode.Include("Video").Where(e => e.SeasonId == episode.SeasonId).Where(e => e.EpisodeNumber > episode.EpisodeNumber).OrderBy(e => e.EpisodeNumber).FirstOrDefaultAsync();
                Episode? NextEpisode = nextEpisodeCurrentSeason ?? await _context.Episode.Include("Video").Where(e => e.Season.ShowId == show.Id).Where(e => e.Season.SeasonNumber > season.SeasonNumber).OrderBy(e => e.Season.SeasonNumber).ThenBy(e => e.EpisodeNumber).FirstOrDefaultAsync();
                Console.WriteLine("NextEpisode.Video: " + NextEpisode?.Video.Id);
                ViewData["NextEpisode"] = NextEpisode;
            }

            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        //GET: Subtitles/5/eng

        public async Task<IActionResult> Subtitles(int? id, string? lang)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Video.FirstOrDefaultAsync(v => v.Id == id);

            if (video == null)
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(video.Filepath))
            {
                return NotFound();
            }


            string parentFilepath = string.Join("\\", video.Filepath.Split("\\").SkipLast(1));
            string? filename = string.Join("", video.Filepath.Split("\\").Last().SkipLast(4));
            Console.Write("Filename: " + filename);

            string? vttSubs = Directory.GetFiles(parentFilepath, "*.vtt", SearchOption.AllDirectories).Where(s => s.ToLower().Contains(lang.ToLower())).Where(s => s.ToLower().Contains(filename.ToLower())).FirstOrDefault();
            string? srtSubs = Directory.GetFiles(parentFilepath, "*.srt", SearchOption.AllDirectories).Where(s => s.ToLower().Contains(lang.ToLower())).Where(s => s.ToLower().Contains(filename.ToLower())).FirstOrDefault();

            if (!vttSubs.IsNullOrEmpty())
            {
                return File(System.IO.File.OpenRead(vttSubs!), "text/vtt");
            }
            else if (!srtSubs.IsNullOrEmpty())
            {
                return File(System.IO.File.OpenRead(srtSubs!), "text/plain");
            }

            return NotFound();

        }
        public async Task<IActionResult> Stream(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Video.FirstOrDefaultAsync(v => v.Id == id);

            if (video == null)
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(video.Filepath))
            {
                return NotFound();
            }

            var fileInfo = new FileInfo(video.Filepath);
            long fileLength = fileInfo.Length;
            var requestHeaders = Request.GetTypedHeaders();
            var responseHeaders = Response.GetTypedHeaders();
            var range = requestHeaders.Range;

            if (range == null)
            {
                return File(System.IO.File.OpenRead(video.Filepath), "video/mp4", enableRangeProcessing: false);
            }



            RangeItem rangeItem = new RangeItem(range.ToString());

            var lastModified = fileInfo.LastWriteTimeUtc;
            var entityTag = new EntityTagHeaderValue($"\"{fileInfo.LastWriteTimeUtc.Ticks}\"");

            responseHeaders.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(3600)
            };
            responseHeaders.LastModified = lastModified;
            responseHeaders.ETag = entityTag;


            if (requestHeaders.IfModifiedSince.HasValue && requestHeaders.IfModifiedSince.Value >= lastModified)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            if (requestHeaders.IfNoneMatch != null && requestHeaders.IfNoneMatch.Contains(entityTag))
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            const int CHUNK_SIZE = 1024 * 1024;
            var start = rangeItem.From ?? 0;
            var end = Math.Min(start + CHUNK_SIZE, fileLength - 1);

            var contentLength = end - start + 1;
            var contentRange = new ContentRangeHeaderValue(start, end, fileLength);

            Response.StatusCode = StatusCodes.Status206PartialContent;
            responseHeaders.ContentRange = contentRange;


            var fileStream = new FileStream(video.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(start, SeekOrigin.Begin);


            return File(fileStream, "video/mp4", fileDownloadName: fileInfo.Name, lastModified, entityTag, enableRangeProcessing: true);


        }

        struct RangeItem
        {

            public RangeItem(string r)
            {
                string[] range = r.Replace("bytes=", "").Split("-");
                From = long.Parse(range[0]);
                To = long.Parse(range[0]);

            }
            public long? From;
            public long? To;
        }
    }
}
