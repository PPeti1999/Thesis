using HealthyAPI.DTOs.MealFoods;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealFoodsController : ControllerBase
    {
        private readonly IMealFoodsService _service;

        public MealFoodsController(IMealFoodsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealFoodResponseDto>>> GetAll()
        {
            var items = await _service.GetAllMealFoods();
            return Ok(items.Select(mf => new MealFoodResponseDto
            {
                MealFoodID = mf.MealFoodID,
                MealEntryID = mf.MealEntryID,
                FoodID = mf.FoodID,
                FoodName = mf.Food?.Title,
                Quantity = mf.Quantity
            }));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealFoodResponseDto>> GetById(string id)
        {
            var mf = await _service.GetByIdMealFoods(id);
            if (mf == null) return NotFound();

            return Ok(new MealFoodResponseDto
            {
                MealFoodID = mf.MealFoodID,
                MealEntryID = mf.MealEntryID,
                FoodID = mf.FoodID,
                FoodName = mf.Food?.Title,
                Quantity = mf.Quantity
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealFoodResponseDto>> Create(MealFoodCreateDto dto)
        {
            var entity = new MealFoods
            {
                MealFoodID = Guid.NewGuid().ToString(),
                MealEntryID = dto.MealEntryID,
                FoodID = dto.FoodID,
                Quantity = dto.Quantity
            };

            var created = await _service.CreateMealFoods(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.MealFoodID }, new MealFoodResponseDto
            {
                MealFoodID = created.MealFoodID,
                MealEntryID = created.MealEntryID,
                FoodID = created.FoodID,
                FoodName = created.Food?.Title,
                Quantity = created.Quantity
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealFoodResponseDto>> Update(string id, MealFoodCreateDto dto)
        {
            var updated = new MealFoods
            {
                MealEntryID = dto.MealEntryID,
                FoodID = dto.FoodID,
                Quantity = dto.Quantity
            };

            var result = await _service.UpdateMealFoods(id, updated);
            if (result == null) return NotFound();

            return Ok(new MealFoodResponseDto
            {
                MealFoodID = result.MealFoodID,
                MealEntryID = result.MealEntryID,
                FoodID = result.FoodID,
                FoodName = result.Food?.Title,
                Quantity = result.Quantity
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteMealFoods(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

