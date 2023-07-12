using Microsoft.AspNetCore.Mvc;
using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using DataProvider.Infrastructure.Exceptions;

namespace DataProvider.API.Controllers;

[ApiController]
[Route("v1/recipes")]
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

    [HttpGet("ByTitle/{partialTitle}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Recipe>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipesByTitle(string partialTitle)
    {
        try
        {
            var recipes = await _recipeRepository.GetRecipesByTitle(partialTitle);
            _logger.LogInformation("GetRecipesByTitle returns {@Recipes}", recipes);

            return Ok(recipes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{uuid:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Recipe))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipeByUuid(Guid uuid)
    {
        try
        {
            var recipe = await _recipeRepository.GetRecipeByUuid(uuid); 
            _logger.LogInformation("GetRecipeByUuid returns {@Recipe}", recipe);
        
            if (recipe == null)
            {
                throw new RecipeDoesNotExistException();
            }

            return Ok(recipe);
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@RecipeUuid}' does not exist", uuid);
            return StatusCode(404, $"Recipe '{uuid}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Recipe))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRecipe(CreateRecipeDto recipeDto)
    {
        try
        {
            var recipe = await _recipeRepository.CreateRecipe(recipeDto);
            _logger.LogInformation("CreateRecipe gets {@Dto} returns {@Recipe}", recipeDto, recipe);

            return CreatedAtAction(
                nameof(GetRecipes), 
                new {uuid = recipe.Uuid, title = recipe.Title}, 
                recipe);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRecipe(UpdateRecipeDto recipeDto)
    {
        try
        {
            var response = await _recipeRepository.UpdateRecipe(recipeDto);

            if (response == 0)
            {
                throw new RecipeDoesNotExistException();
            }
            
            _logger.LogInformation("Updated recipe {@Title}", recipeDto.Title);
            return NoContent();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@RecipeUuid}' does not exist", recipeDto.Uuid);
            return StatusCode(404, $"Recipe '{recipeDto.Uuid}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRecipe(Guid uuid)
    {
        try
        {
            var response = await _recipeRepository.DeleteRecipe(uuid);

            if (response == 0)
                throw new RecipeDoesNotExistException();
            
            return NoContent();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@Uuid}' does not exist", uuid);
            return StatusCode(404, $"Recipe '{uuid}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }
}