using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataProvider.API.Controllers;

[ApiController]
[Route("v1/ingredients")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<IngredientController> _logger;
    
    public IngredientController(IIngredientRepository ingredientRepository, ILogger<IngredientController> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }
    
    [HttpGet("{recipeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Ingredient>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipeIngredientsByRecipeId(Guid recipeId)
    {
        try
        {
            var ingredients = await _ingredientRepository.GetRecipeIngredientsByRecipeIdAsync(recipeId);
            _logger.LogInformation("[DP]: GetRecipeIngredientsByRecipeId returns {@Ingredients}", ingredients);
            
            return Ok(ingredients);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }
}