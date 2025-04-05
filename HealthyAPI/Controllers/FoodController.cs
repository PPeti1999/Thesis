using HealthyAPI.Models;
using HealthyAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HealthyAPI.Controllers
{

        [Route("api/[controller]")]
        [ApiController]
        public class FoodController : Controller
        {
            private readonly IFoodRepository _foodRepository;
            private readonly IPhotoRepository photoRepository;
            private readonly ILogger<FoodController> _logger;

            public FoodController(IPhotoRepository photoRepository, IFoodRepository foodRepository, ILogger<FoodController> logger)
            {
                this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

                this._foodRepository = foodRepository ?? throw new ArgumentNullException(nameof(foodRepository));
                this.photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));

            }

            // GET: api/Food
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Food>>> GetAllFoods()
            {
                var foods = await _foodRepository.ListFoods();
                return Ok(foods);
            }

            // GET: api/Food/{id}
            [HttpGet("{id}")]
            public async Task<ActionResult<Food>> GetFood(string id)
            {
                var food = await _foodRepository.GetFood(id);
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
                    var uploadedPhoto = await photoRepository.UploadPhoto(food.Photo);
                    food.PhotoID = uploadedPhoto.PhotoID; // vagy uploadedPhoto.Id, attól függően, hogyan van elnevezve
                }

                var createdFood = await _foodRepository.AddFood(food);
                return Ok(createdFood);
             }

            // PUT: api/Food
            [HttpPut]
            [Authorize]
            public async Task<ActionResult<Food>> UpdateFood([FromBody] Food food)
            {
                if (food == null)
                    return BadRequest();

                var updatedFood = await _foodRepository.UpdateFood(food);
                return Ok(updatedFood);
            }

            // DELETE: api/Food/{id}
            [HttpDelete("{id}")]
            [Authorize]
            public async Task<ActionResult> DeleteFood(string id)
                {
                var food = await _foodRepository.GetFood(id);

                if (food == null)
                {
                    return NotFound();
                }
                var success = await _foodRepository.DeleteFood(id);
                if (!success)
                        return NotFound();
                await photoRepository.DeletePhoto(food.FoodID);// gyakorlathoztartozó kép törlése
                return NoContent();
            }
        }
    
}
