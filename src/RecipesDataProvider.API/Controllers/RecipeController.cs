using Microsoft.AspNetCore.Mvc;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Domain.Interfaces;

namespace RecipesDataProvider.API.Controllers;

[Route("v1/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<RecipeController> _logger;
    
    public RecipeController(IRecipeRepository recipeRepository, ILogger<RecipeController> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Recipe>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipes()
    {
        try
        {
            var recipes = await _recipeRepository.GetRecipes(); 
            _logger.LogInformation("GetRecipes returns {@Recipes}", recipes);
        
            return Ok(recipes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }
}