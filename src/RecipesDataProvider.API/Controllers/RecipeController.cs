using Microsoft.AspNetCore.Mvc;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Domain.Interfaces;

namespace RecipesDataProvider.Controllers;

[Route("v1/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeController(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Recipe>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipes()
    {
        try
        {
            var recipes = await _recipeRepository.GetRecipes(); 
        
            return Ok(recipes);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}