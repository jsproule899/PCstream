using System.Text.RegularExpressions;

namespace MvcMovie.Models;

class MediaFile
{
    public string? Name { get; set; }
    public string Filepath { get; set; }
    public bool isMovie = true;
    public bool isShow = false;

    public int? Season { get; set; } = null;
    public int? Episode { get; set; } = null;

    public MediaFile(string path)
    {
        Filepath = path;
    }

    public void parseNameFromPath()
    {

        int i = this.Filepath.LastIndexOf("\\") + 1;
        string fileSuffixRemoved = this.Filepath.Substring(i).Replace(".mp4", "").Replace(".mkv", "").Replace(".", " ");
        string specialCharsRemoved = RemoveSpecialCharacters(fileSuffixRemoved);

        Regex regSE = new Regex(@"([S][0-9]{2}[E][0-9]{2})", RegexOptions.IgnoreCase);
        Match matchSE = regSE.Match(specialCharsRemoved);

        if (matchSE.Success)
        {
            isShow = true;
            isMovie = false;
            int season = -1;
            int episode = -1;
            int.TryParse(matchSE.Value.ToString().Substring(1, 2), out season);
            
            if (season != -1)
            {
                this.Season = season;
                Console.WriteLine("Seasons number is: " + Season);
            }

            int.TryParse(matchSE.Value.ToString().Substring(4, 2), out episode);
            if (episode != -1)
            {
                this.Episode = episode;
                Console.WriteLine("Episode number is: " + Episode);
            }

            
            this.Name = specialCharsRemoved.Substring(0, matchSE.Index - 1).Trim();
            return;
        }

        Regex regYear = new Regex(@"(19|20[0-9]{2})");
        Match matchYear = regYear.Match(specialCharsRemoved);

        if (matchYear.Success)
        {
            this.Name = specialCharsRemoved.Substring(0, matchYear.Index - 1).Trim();
            return;
        }

        this.Name = specialCharsRemoved;
    }

    public static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9 ]+", "", RegexOptions.Compiled);
    }
}