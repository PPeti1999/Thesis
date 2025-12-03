using HealthyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using HealthyAPI.Services;
using System.Linq;
using HealthyAPI.DTOs.Food;
using HealthyAPI.DTOs.Recipe;

namespace HealthyAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FoodResponseDto>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");
            var results = await _foodService.Search(query);
            return Ok(results);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodResponseDto>>> GetAllFoods()
        {
          var foods = await _foodService.ListFoods();
          return Ok(foods);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodResponseDto>> GetFood(string id)
        {
          var dto = await _foodService.GetFood(id);
          if (dto == null) return NotFound();
          return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FoodResponseDto>> AddFood([FromBody] FoodCreateDto dto)
        {
          var createdDto = await _foodService.AddFood(dto);
          return Ok(createdDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<FoodResponseDto>> UpdateFood(string id, [FromBody] FoodCreateDto dto)
        {
          var updatedDto = await _foodService.UpdateFood(id, dto);
          if (updatedDto == null) return NotFound();
          return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFood(string id)
        {
            try
            {
                var success = await _foodService.DeleteFood(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
