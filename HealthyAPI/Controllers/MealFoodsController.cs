using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealFoodsController : ControllerBase
    {
        private readonly IMealFoodsService _mealFoodsService;

        public MealFoodsController(IMealFoodsService mealFoodsService)
        {
            _mealFoodsService = mealFoodsService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealFoods>>> GetAll()
        {
            var list = await _mealFoodsService.GetAllMealFoods();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealFoods>> GetById(string id)
        {
            var item = await _mealFoodsService.GetByIdMealFoods(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("byMealEntry/{mealEntryId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealFoods>>> GetByMealEntryId(string mealEntryId)
        {
            var items = await _mealFoodsService.GetMealFoodsByMealEntryId(mealEntryId);
            if (items == null || !items.Any()) return NotFound();
            return Ok(items);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealFoods>> Create(MealFoods mealFood)
        {
            var created = await _mealFoodsService.CreateMealFoods(mealFood);

            // ✨ Frissítjük az étkezéshez tartozó tápértékeket
            await _mealFoodsService.RecalculateMealEntryNutrition(mealFood.MealEntryID);

            return CreatedAtAction(nameof(GetById), new { id = created.MealFoodID }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, MealFoods mealFood)
        {
            var result = await _mealFoodsService.UpdateMealFoods(id, mealFood);
            if (result == null) return NotFound();

            // ✨ Frissítjük az étkezéshez tartozó tápértékeket
            await _mealFoodsService.RecalculateMealEntryNutrition(mealFood.MealEntryID);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var mealFood = await _mealFoodsService.GetByIdMealFoods(id);
            if (mealFood == null) return NotFound();

            var result = await _mealFoodsService.DeleteMealFoods(id);
            if (!result) return NotFound();

            // ✨ Frissítjük az étkezéshez tartozó tápértékeket
            await _mealFoodsService.RecalculateMealEntryNutrition(mealFood.MealEntryID);

            return NoContent();
        }
    }
}

