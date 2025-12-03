using HealthyAPI.Data;
using HealthyAPI.DTOs.RecipeFood;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeFoodsController : ControllerBase
    {
        private readonly IRecipeFoodService _service;

        public RecipeFoodsController(IRecipeFoodService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RecipeFoodResponseDto>>> GetAll()
        {
          var items = await _service.GetAll();
          return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> GetById(string id)
        {
          var dto = await _service.GetById(id);
          if (dto == null) return NotFound();

          return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> Create(RecipeFoodCreateDto dto)
        {
          var createdDto = await _service.Create(dto);

          return Ok(createdDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeFoodResponseDto>> Update(string id, RecipeFoodCreateDto dto)
        {
          var updatedDto = await _service.Update(id, dto);

          if (updatedDto == null) return NotFound();

          return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.Delete(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
