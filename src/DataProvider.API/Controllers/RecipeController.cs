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
            _logger.LogInformation("[DP]: GetRecipes returns {@Recipes}", recipes);
        
            return Ok(recipes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
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
            _logger.LogInformation("[DP]: GetRecipesByTitle returns {@Recipes}", recipes);

            return Ok(recipes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Recipe))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipeWithIngredientsById(Guid id)
    {
        try
        {
            var recipe = await _recipeRepository.GetRecipeWithIngredientsById(id); 
            _logger.LogInformation("[DP]: GetRecipeWithIngredientsById returns {@Recipe}", recipe);
        
            if (recipe == null)
            {
                throw new RecipeDoesNotExistException();
            }

            return Ok(recipe);
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "[DP]: Recipe '{@RecipeId}' does not exist", id);
            return StatusCode(404, $"Recipe '{id}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:guid}/basic-info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Recipe))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecipeById(Guid id)
    {
        try
        {
            var recipe = await _recipeRepository.GetRecipeById(id); 
            _logger.LogInformation("[DP]: GetRecipeById returns {@Recipe}", recipe);
        
            if (recipe == null)
            {
                throw new RecipeDoesNotExistException();
            }
    
            return Ok(recipe);
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "[DP]: Recipe '{@RecipeId}' does not exist", id);
            return StatusCode(404, $"Recipe '{id}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
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
            _logger.LogInformation("[DP]: CreateRecipe gets {@Dto} returns {@Recipe}", recipeDto, recipe);

            return CreatedAtAction(
                nameof(GetRecipes), 
                new {id = recipe.Id, title = recipe.Title}, 
                recipe);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
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
            
            _logger.LogInformation("[DP]: Updated recipe {@Title}", recipeDto.Title);
            return NoContent();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "[DP]: Recipe '{@RecipeId}' does not exist", recipeDto.Id);
            return StatusCode(404, $"Recipe '{recipeDto.Id}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRecipe(Guid id)
    {
        try
        {
            var response = await _recipeRepository.DeleteRecipe(id);

            if (response == 0)
                throw new RecipeDoesNotExistException();
            
            return NoContent();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "[DP]: Recipe '{@Id}' does not exist", id);
            return StatusCode(404, $"Recipe '{id}' does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DP]: Communication with repository failed");
            return StatusCode(500, e.Message);
        }
    }
}