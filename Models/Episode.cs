using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models;

public class Episode
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title { get; set; }

    [Required]
    public int? EpisodeNumber { get; set; }

    [Required]
    public int? SeasonNumber { get; set; }

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Required]
    public double? Rating { get; set; }

    [Required]
    public double? Runtime { get; set; }

    public string? Poster { get; set; }

    public string? Summary { get; set; }

    [Required]
    public Video Video { get; set; }

    public int? SeasonId { get; set; } = null;

    public Season? Season { get; set; } = null;
}