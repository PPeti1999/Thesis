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
        [HttpGet("by-meal-entry/{mealEntryId}")]
        public async Task<ActionResult<IEnumerable<MealFoodResponseDto>>> GetByMealEntry(string mealEntryId)
        {
            var result = await _service.GetByMealEntryIdAsync(mealEntryId);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealFoodResponseDto>>> GetAll()
        {
            var items = await _service.GetAllMealFoods();
            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealFoodResponseDto>> GetById(string id)
        {
            var dto = await _service.GetByIdMealFoods(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealFoodResponseDto>> Create(MealFoodCreateDto dto)
        {
            var created = await _service.CreateMealFoods(dto);
            return Ok(created);
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealFoodResponseDto>> Update(string id, MealFoodCreateDto dto)
        {
            var updated = await _service.UpdateMealFoods(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
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

