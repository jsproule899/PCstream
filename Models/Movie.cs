using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models;

public class Movie
{
    public int Id {get; set;}
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title {get; set;}

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate {get; set;}

    
    public string[]? Genres {get; set;}

    
    public string? TmdbId {get; set;}
    
    
    [Required]
    public double? Rating {get; set;}

      
    [Required]
    public double? Runtime {get; set;}

     
    public string? Poster {get; set;}
     
       
    public string? Summary {get; set;}

    
    public Video? Video {get; set;}
}