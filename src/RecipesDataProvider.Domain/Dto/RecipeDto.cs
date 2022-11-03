using System.ComponentModel.DataAnnotations;

namespace RecipesDataProvider.Domain.Dto;

public class RecipeDto
{
    [Required]
    public string? Title { get; set; }
}