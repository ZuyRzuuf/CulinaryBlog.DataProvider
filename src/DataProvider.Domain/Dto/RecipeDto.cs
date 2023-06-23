using System.ComponentModel.DataAnnotations;

namespace DataProvider.Domain.Dto;

public class RecipeDto
{
    [Required]
    public string? Title { get; set; }
}