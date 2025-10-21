using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMovie.Models;

public class Show
{
    public int Id {get; set;}
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title {get; set;}

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate {get; set;}

    public int? NumberOfSeasons {get; set;}

    public int? TmdbId {get; set;}
 
    public string? Poster {get; set;}
     
    public string? Summary {get; set;}
    
    [Required]
    public double? Rating {get; set;}

    public string[]? Genres {get; set;}

    [InverseProperty("Show")]
    public ICollection<Season> Seasons {get; set; } = new List<Season>();

}