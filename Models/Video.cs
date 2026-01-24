using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models;

public class Video
{
    [Required]
    public int Id { get; set; }


    [Required]
    public required string Filepath { get; set; }


}