using System.ComponentModel.DataAnnotations;

namespace DataProvider.Domain.Dto;

public class UpdateRecipeDto : RecipeDto
{
    [Required]
    public Guid Id { get; set; }
}