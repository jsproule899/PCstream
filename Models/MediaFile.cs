using System.Text.RegularExpressions;

namespace MvcMovie.Models;

partial class MediaFile(string path)
{
    public string Name { get; set; } = Path.GetFileNameWithoutExtension(path);
    public string Filepath { get; set; } = path;
    public bool isMovie = true;
    public bool isShow = false;

    public int? Season { get; set; } = null;
    public int? Episode { get; set; } = null;

    public void ParseNameFromPath()
    {

        int i = this.Filepath.LastIndexOf('\\') + 1;
        string fileSuffixRemoved = this.Filepath[i..].Replace(".mp4", "").Replace(".mkv", "").Replace(".", " ");
        string specialCharsRemoved = RemoveSpecialCharacters(fileSuffixRemoved);

        Regex regSE = SeasonEpisodeRegex();
        Match matchSE = regSE.Match(specialCharsRemoved);

        if (matchSE.Success)
        {
            isShow = true;
            isMovie = false;
            _ = int.TryParse(matchSE.Value.ToString().AsSpan(1, 2), out int season);

            if (season != -1)
            {
                this.Season = season;
                Console.WriteLine("Seasons number is: " + Season);
            }

            _ = int.TryParse(matchSE.Value.ToString().AsSpan(4, 2), out int episode);
            if (episode != -1)
            {
                this.Episode = episode;
                Console.WriteLine("Episode number is: " + Episode);
            }

            int seasonEpisdoeIndex = matchSE.Index == 0 ? 0 : matchSE.Index - 1;
            this.Name = specialCharsRemoved[..seasonEpisdoeIndex].Trim();
            return;
        }

        Regex regYear = YearRegex();
        Match matchYear = regYear.Match(specialCharsRemoved);

        if (matchYear.Success)
        {
            int yearIndex = matchYear.Index == 0 ? 0 : matchYear.Index - 1;
            this.Name = specialCharsRemoved[..yearIndex].Trim();
            return;
        }

        this.Name = specialCharsRemoved;
    }

    public static string RemoveSpecialCharacters(string str)
    {
        return SpecialCharRegex().Replace(str, "");
    }

    [GeneratedRegex(@"(19|20[0-9]{2})")]
    private static partial Regex YearRegex();

    [GeneratedRegex(@"([S][0-9]{2}[E][0-9]{2})", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SeasonEpisodeRegex();

    [GeneratedRegex("[^a-zA-Z0-9 ]+", RegexOptions.Compiled)]
    private static partial Regex SpecialCharRegex();
}