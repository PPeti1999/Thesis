using HealthyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using HealthyAPI.Services;

namespace HealthyAPI.Controllers
{

        [Route("api/[controller]")]
        [ApiController]
        public class FoodController : Controller
        {
            private readonly IFoodService _foodService;
        private readonly IPhotoService photoService;
            private readonly ILogger<FoodController> _logger;

            public FoodController(IPhotoService photoService, IFoodService foodService, ILogger<FoodController> logger)
            {
                this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _foodService = foodService ?? throw new ArgumentNullException(nameof(foodService));
            this.photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));

            }

            // GET: api/Food
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Food>>> GetAllFoods()
            {
                var foods = await _foodService.ListFoods();
                return Ok(foods);
            }

            // GET: api/Food/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<Food>> GetFood(string id)
            {
                var food = await _foodService.GetFood(id);
                if (food == null)
                    return NotFound();
                return Ok(food);
            }

            // POST: api/Food
            [HttpPost]
            [Authorize]
            public async Task<ActionResult<Food>> AddFood([FromBody] Food food)
        {
                if(food.Photo == null) return BadRequest();
                if (food == null) return BadRequest();

                if (food.Photo != null)
                {
                    var uploadedPhoto = await photoService.UploadPhoto(food.Photo);
                    food.PhotoID = uploadedPhoto.PhotoID; // vagy uploadedPhoto.Id, attól függően, hogyan van elnevezve
                }

                var createdFood = await _foodService.AddFood(food);
                return Ok(createdFood);
             }

            // PUT: api/Food
            [HttpPut]
            [Authorize]
            public async Task<ActionResult<Food>> UpdateFood([FromBody] Food food)
            {
                if (food == null)
                    return BadRequest();

                var updatedFood = await _foodService.UpdateFood(food);
                return Ok(updatedFood);
            }

            // DELETE: api/Food/{id}
            [HttpDelete("{id}")]
            [Authorize]
            public async Task<ActionResult> DeleteFood(string id)
                {
                var food = await _foodService.GetFood(id);

                if (food == null)
                {
                    return NotFound();
                }
                var success = await _foodService.DeleteFood(id);
                if (!success)
                        return NotFound();
                await photoService.DeletePhoto(food.PhotoID);// gyakorlathoztartozó kép törlése
                return NoContent();
            }
        }
    
}
