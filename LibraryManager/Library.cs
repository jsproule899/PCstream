using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MvcMovie.Migrations;


namespace LibraryManager;

class Library
{
    string Path { get; set; }
    static HttpClient client = new HttpClient();

    public Library(string path)
    {
        Path = path;
    }
    public async void Scan(MvcMovieContext context)
    {
        string[] mp4Files = Directory.GetFiles(Path, "*.mp4", SearchOption.AllDirectories);
        string[] mkvFiles = Directory.GetFiles(Path, "*.mkv", SearchOption.AllDirectories);
        string[] files = mp4Files.Concat(mkvFiles).ToArray();




        foreach (string file in files)
        {
            MediaFile mediaFile = new MediaFile(file);
            mediaFile.parseNameFromPath();

            if (mediaFile.isMovie)
            {
                await AddMovie(context, mediaFile);

            }
            else if (mediaFile.isShow)
            {
                await addShow(context, mediaFile);

            }
        }

        context.Video.ToList().ForEach(video =>
        {
            if (!File.Exists(video.Filepath))
            {
                Movie? movie = context.Movie.Include(m => m.Video).Where(movie => movie.Video == video).FirstOrDefault();
                if (movie != null)
                {
                    context.Remove(movie);
                }

                Episode? episode = context.Episode.Include(e => e.Video).Where(e => e.Video == video).FirstOrDefault();
                if (episode != null)
                {
                    context.Remove(episode);
                }
                context.Season.Include(s => s.Episodes).Where(s=> !s.Episodes.Any()).ToList().ForEach(season =>{
                     context.Remove(season);
                });

              
               context.Show.Include(s => s.Seasons).Where(s=> !s.Seasons.Any()).ToList().ForEach(show =>{
                     context.Remove(show);
                });

                context.Remove(video);
                context.SaveChanges();
            }
        });







    }


    private static async Task addShow(MvcMovieContext context, MediaFile mediaFile)
    {


        TmdbShow show = new TmdbShow();
        int showTmdbId;
        HttpClient httpClient = new HttpClient();

        if (context.Video.Where(v => v.Filepath.Equals(mediaFile.Filepath)).ToList().IsNullOrEmpty())
        {

            HttpResponseMessage response = await client.GetAsync("https://api.themoviedb.org/3/search/tv?api_key=d0f5aebbfd72d42d2d77d80d3997aefd&query=" + mediaFile.Name);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                TmdbResult results = new TmdbResult();
                results = JsonConvert.DeserializeObject<TmdbResult>(json);
                string output = JsonConvert.SerializeObject(results.results);



                List<TmdbId> ids = JsonConvert.DeserializeObject<List<TmdbId>>(output);

                if (!ids.IsNullOrEmpty())
                {
                    showTmdbId = ids[0].id;
                    Console.WriteLine(" TV Show Id is: " + showTmdbId);
                    if (context.Show.Where(s => s.TmdbId.Equals(showTmdbId)).ToList().IsNullOrEmpty())
                    {
                        response = await client.GetAsync("https://api.themoviedb.org/3/tv/" + showTmdbId + "?api_key=d0f5aebbfd72d42d2d77d80d3997aefd");
                        if (response.IsSuccessStatusCode)
                        {

                            string showJson = await response.Content.ReadAsStringAsync();
                            show = JsonConvert.DeserializeObject<TmdbShow>(showJson);


                        }
                        var newShow = new Show
                        {
                            Title = show.name ?? mediaFile.Name,
                            ReleaseDate = DateTime.Parse(show.first_air_date ?? "1900-01-01"),
                            NumberOfSeasons = show.number_of_seasons,
                            Rating = show.vote_average,
                            Genres = ["TBD"],
                            TmdbId = showTmdbId,
                            Summary = show.overview,
                            Poster = show.poster_path,

                        };
                        context.Show.Add(newShow);
                        await context.SaveChangesAsync();

                    }



                    Show curShow = context.Show.Include("Seasons").Where(show => show.TmdbId.Equals(showTmdbId)).ToList().FirstOrDefault();
                    if (curShow != null)
                    {
                        if (curShow.Seasons.Where(season => season.SeasonNumber.Equals(mediaFile.Season)).ToList().IsNullOrEmpty())
                        {
                            response = await client.GetAsync("https://api.themoviedb.org/3/tv/" + showTmdbId + "/season/" + mediaFile.Season + "?api_key=d0f5aebbfd72d42d2d77d80d3997aefd");
                            if (response.IsSuccessStatusCode)
                            {
                                string seasonJson = await response.Content.ReadAsStringAsync();
                                TmdbSeason season = JsonConvert.DeserializeObject<TmdbSeason>(seasonJson);

                                var newSeason = new Season
                                {
                                    Title = season.name ?? mediaFile.Season.ToString(),
                                    Rating = season.vote_average,
                                    SeasonNumber = season.season_number,
                                    Summary = season.overview,
                                    Poster = season.poster_path,
                                    ShowId = curShow.Id
                                };


                                context.Season.Add(newSeason);
                                await context.SaveChangesAsync();

                            }
                        }
                    }



                    Season curSeason = context.Season.Include("Episodes").Where(season => season.Show.TmdbId.Equals(showTmdbId)).Where(season => season.SeasonNumber.Equals(mediaFile.Season)).ToList().FirstOrDefault();
                    if (curSeason != null)
                    {

                        if (curSeason.Episodes.Where(episode => episode.EpisodeNumber.Equals(mediaFile.Episode)).ToList().IsNullOrEmpty())
                        {
                            response = await client.GetAsync("https://api.themoviedb.org/3/tv/" + showTmdbId + "/season/" + mediaFile.Season + "/episode/" + mediaFile.Episode + "?api_key=d0f5aebbfd72d42d2d77d80d3997aefd");
                            if (response.IsSuccessStatusCode)
                            {
                                string episodeJson = await response.Content.ReadAsStringAsync();
                                TmdbEpisode episode = JsonConvert.DeserializeObject<TmdbEpisode>(episodeJson);

                                var newEpisode = new Episode
                                {
                                    Title = episode.name ?? mediaFile.Season.ToString(),
                                    EpisodeNumber = mediaFile.Episode,
                                    ReleaseDate = DateTime.Parse(episode.air_date),
                                    Rating = episode.vote_average,
                                    SeasonNumber = episode.season_number,
                                    Summary = episode.overview,
                                    Runtime = episode.runtime ?? 0,
                                    Poster = episode.still_path,
                                    Video = new Video()
                                    {
                                        Filepath = mediaFile.Filepath
                                    },
                                    SeasonId = curSeason.Id
                                };


                                context.Episode.Add(newEpisode);
                                await context.SaveChangesAsync();

                            }

                        }


                    }

                }
            }
        }
    }



    private static async Task AddMovie(MvcMovieContext context, MediaFile mediaFile)
    {
        TmdbMovie movie = new TmdbMovie();
        HttpClient httpClient = new HttpClient();
        TmdbResult results = new TmdbResult();
        List<TmdbId> ids = null!;
        HttpResponseMessage response;



        if (context.Video.Where(v => v.Filepath.Equals(mediaFile.Filepath)).ToList().IsNullOrEmpty())
        {
            string query = mediaFile.Name;
            while (results.results.IsNullOrEmpty() && !query.Equals(string.Empty))
            {
                int lastTokenIndex = query.LastIndexOf(" ");
                response = await client.GetAsync("https://api.themoviedb.org/3/search/movie?api_key=d0f5aebbfd72d42d2d77d80d3997aefd&query=" + query);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    results = JsonConvert.DeserializeObject<TmdbResult>(json);
                    if (!results.results.IsNullOrEmpty())
                    {
                        string output = JsonConvert.SerializeObject(results.results);
                        ids = JsonConvert.DeserializeObject<List<TmdbId>>(output);
                    }
                    else if (lastTokenIndex > 0)
                    {

                        query = query.Substring(0, lastTokenIndex).Trim();
                    }
                    else if (lastTokenIndex < 0)
                    {
                        Console.WriteLine("Media File is not a recognised Movie or TV Show");
                        break;
                    }

                }

            }

            Console.WriteLine("out of loop");

            if (!ids.IsNullOrEmpty())
            {
                Console.WriteLine("Movie Id is: " + ids[0].id);

                response = await client.GetAsync("https://api.themoviedb.org/3/movie/" + ids[0].id + "?api_key=d0f5aebbfd72d42d2d77d80d3997aefd");
                if (response.IsSuccessStatusCode)
                {

                    string movieJson = await response.Content.ReadAsStringAsync();
                    movie = JsonConvert.DeserializeObject<TmdbMovie>(movieJson);

                    context.Movie.Add(
                       new Movie
                       {
                           Title = movie.title ?? mediaFile.Name,
                           ReleaseDate = DateTime.Parse(movie.release_date ?? "1900-01-01"),
                           Rating = movie.vote_average,
                           Genres = ["TBD"],
                           Runtime = movie.runtime,
                           Summary = movie.overview,
                           Poster = movie.poster_path,
                           TmdbId = ids[0].id.ToString(),
                           Video = new Video()
                           {
                               Filepath = mediaFile.Filepath
                           }

                       }
                   );

                    context.SaveChanges();

                }
            }
        }
    }
}







struct TmdbId
{
    [JsonPropertyName("id")]
    public int id;
}

struct TmdbMovie
{
    [JsonPropertyName("title")]
    public string title;
    [JsonPropertyName("release_date")]
    public string release_date;
    [JsonPropertyName("runtime")]
    public int runtime;
    [JsonPropertyName("overview")]
    public string overview;
    [JsonPropertyName("poster_path")]
    public string poster_path;

    [JsonPropertyName("vote_average")]
    public double vote_average;


}

struct TmdbShow
{
    [JsonPropertyName("id")]
    public int id;
    [JsonPropertyName("name")]
    public string name;
    [JsonPropertyName("first_air_date")]
    public string first_air_date;
    [JsonPropertyName("number_of_seasons")]
    public int number_of_seasons;
    [JsonPropertyName("number_of_episodes")]
    public int number_of_episodes;
    [JsonPropertyName("overview")]
    public string overview;
    [JsonPropertyName("poster_path")]
    public string poster_path;

    [JsonPropertyName("vote_average")]
    public double vote_average;

}

struct TmdbSeason
{
    [JsonPropertyName("id")]
    public int id;
    [JsonPropertyName("name")]
    public string name;
    [JsonPropertyName("air_date")]
    public string air_date;
    [JsonPropertyName("season_number")]
    public int season_number;
    [JsonPropertyName("number_of_episodes")]
    public string overview;
    [JsonPropertyName("poster_path")]
    public string poster_path;

    [JsonPropertyName("vote_average")]
    public double vote_average;

}

struct TmdbEpisode
{
    [JsonPropertyName("id")]
    public int id;
    [JsonPropertyName("name")]
    public string name;
    [JsonPropertyName("air_date")]
    public string air_date;
    [JsonPropertyName("season_number")]
    public int season_number;
    [JsonPropertyName("episode_number")]
    public int episode_number;
    [JsonPropertyName("overview")]
    public string overview;
    [JsonPropertyName("still_path")]
    public string still_path;
    [JsonPropertyName("runtime")]
    public int? runtime;
    [JsonPropertyName("vote_average")]
    public double vote_average;

}

struct TmdbResult
{
    [JsonPropertyName("page")]
    public string page;
    [JsonPropertyName("results")]
    public JArray results;

}

