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

namespace HealthyAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly IPhotoService _photoService;

        public FoodController(IPhotoService photoService, IFoodService foodService)
        {
            _foodService = foodService;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodResponseDto>>> GetAllFoods()
        {
            var foods = await _foodService.ListFoods();
            return Ok(foods.Select(f => new FoodResponseDto
            {
                FoodID = f.FoodID,
                Title = f.Title,
                Protein = f.Protein,
                Fat = f.Fat,
                Carb = f.Carb,
                Calorie = f.Calorie,
                Gram = f.Gram,
                PhotoID = f.PhotoID,
                PhotoData = f.Photo?.PhotoData,
                CreatedAt = f.CreatedAt
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodResponseDto>> GetFood(string id)
        {
            var food = await _foodService.GetFood(id);
            if (food == null) return NotFound();

            return Ok(new FoodResponseDto
            {
                FoodID = food.FoodID,
                Title = food.Title,
                Protein = food.Protein,
                Fat = food.Fat,
                Carb = food.Carb,
                Calorie = food.Calorie,
                Gram = food.Gram,
                PhotoID = food.PhotoID,
                PhotoData = food.Photo?.PhotoData,
                CreatedAt = food.CreatedAt
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FoodResponseDto>> AddFood([FromBody] FoodCreateDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.PhotoData)) return BadRequest();

            var uploadedPhoto = await _photoService.UploadPhoto(new Photo { PhotoData = dto.PhotoData });

            var food = new Food
            {
                FoodID = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Protein = dto.Protein,
                Fat = dto.Fat,
                Carb = dto.Carb,
                Calorie = dto.Calorie,
                Gram = dto.Gram,
                PhotoID = uploadedPhoto.PhotoID,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _foodService.AddFood(food);

            return Ok(new FoodResponseDto
            {
                FoodID = created.FoodID,
                Title = created.Title,
                Protein = created.Protein,
                Fat = created.Fat,
                Carb = created.Carb,
                Calorie = created.Calorie,
                Gram = created.Gram,
                PhotoID = created.PhotoID,
                PhotoData = uploadedPhoto.PhotoData,
                CreatedAt = created.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<FoodResponseDto>> UpdateFood(string id, [FromBody] FoodUpdateDto dto)
        {
            var food = await _foodService.GetFood(id);
            if (food == null) return NotFound();

            food.Title = dto.Title;
            food.Protein = dto.Protein;
            food.Fat = dto.Fat;
            food.Carb = dto.Carb;
            food.Calorie = dto.Calorie;
            food.Gram = dto.Gram;

            if (!string.IsNullOrEmpty(dto.PhotoData))
            {
                var uploadedPhoto = await _photoService.UploadPhoto(new Photo { PhotoData = dto.PhotoData });
                food.PhotoID = uploadedPhoto.PhotoID;
            }

            var updated = await _foodService.UpdateFood(id, food);

            return Ok(new FoodResponseDto
            {
                FoodID = updated.FoodID,
                Title = updated.Title,
                Protein = updated.Protein,
                Fat = updated.Fat,
                Carb = updated.Carb,
                Calorie = updated.Calorie,
                Gram = updated.Gram,
                PhotoID = updated.PhotoID,
                PhotoData = updated.Photo?.PhotoData,
                CreatedAt = updated.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFood(string id)
        {
            var food = await _foodService.GetFood(id);
            if (food == null) return NotFound();

            await _photoService.DeletePhoto(food.PhotoID);
            var success = await _foodService.DeleteFood(id);

            if (!success) return NotFound();

            return NoContent();
        }
    }
}
