using System.ComponentModel.DataAnnotations;

namespace RecipesDataProvider.Domain.Dto;

public class UpdateRecipeDto : RecipeDto
{
    [Required]
    public Guid Uuid { get; set; }
}