using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Lab2.Warehouse.Entities;
using Lab2.Warehouse.Models;
using Lab2.Warehouse.Repositories.Abstractions;

namespace Lab2.Warehouse.Controllers.Api {
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase {
        private readonly IRepository<Ingredient> _ingredients;

        public IngredientsController(IRepository<Ingredient> ingredients) {
            _ingredients = ingredients;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Ingredient[]), StatusCodes.Status200OK)]
        public async Task<ActionResult<Ingredient[]>> GetAsync() {
            return (await _ingredients.GetAllAsync()).ToArray();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Ingredient), StatusCodes.Status200OK)]
        public async Task<ActionResult<Ingredient>> GetByIdAsync(Guid id) {
            return await _ingredients.GetByIdAsync(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Ingredient), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Ingredient>> PostAsync(CreateIngredientModel createIngredientModel) {
            var ingredient = createIngredientModel.Adapt<Ingredient>();
            return await _ingredients.InsertAsync(ingredient);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(Guid id) {
            await _ingredients.DeleteByIdAsync(id);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Ingredient>> PutAsync(Guid id, PutIngredientModel putIngredientModel) {
            var ingredient = putIngredientModel.Adapt<Ingredient>();
            ingredient.Id = id;
            await _ingredients.UpdateByIdAsync(id, IngredientMapping.UpdateExpression(ingredient));
            return ingredient;
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<Ingredient>> PatchAsync(Guid id, JsonPatchDocument<Ingredient> patchDocument) {
            var ingredient = await _ingredients.GetByIdAsync(id);
            patchDocument.ApplyTo(ingredient);
            await _ingredients.UpdateByIdAsync(id, IngredientMapping.UpdateExpression(ingredient));
            return ingredient;
        }
    }
}
