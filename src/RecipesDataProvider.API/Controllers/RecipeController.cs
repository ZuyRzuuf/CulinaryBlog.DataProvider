using Microsoft.AspNetCore.Mvc;
using RecipesDataProvider.Domain.Entities;

namespace RecipesDataProvider.Controllers;

[Route("v1/[controller]")]
public class RecipeController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Recipe>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipes()
    {
        var recipes = new List<Recipe>()
        {
            new() {Uuid = Guid.NewGuid(), Title = "Recipe 1"},
            new() {Uuid = Guid.NewGuid(), Title = "Recipe 2"},
            new() {Uuid = Guid.NewGuid(), Title = "Recipe 3"},
        };
        
        return Ok(recipes);
    }
}