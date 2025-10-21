using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MvcMovie.Models;

public class Season
{
    public int Id { get; set; }

    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title { get; set; }

    [Required]
    public int? SeasonNumber { get; set; }

    public string? Poster { get; set; }

    public string? Summary { get; set; }

    [Required]
    public double? Rating { get; set; }

    [InverseProperty("Season")]
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    public int? ShowId { get; set; }
    public Show? Show { get; set; }

   
}