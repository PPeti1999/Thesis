using HealthyAPI.Data;
using HealthyAPI.DTOs.MealType;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealTypesController : ControllerBase
    {
        private readonly IMealTypeService _service;
        private readonly IPhotoService _photoService;

        public MealTypesController(IMealTypeService service, IPhotoService photoService)
        {
            _service = service;
            _photoService = photoService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MealTypeResponseDto>>> GetAll()
        {
            var items = await _service.GetAll();
            return Ok(items.Select(mt => new MealTypeResponseDto
            {
                MealTypeID = mt.MealTypeID,
                Name = mt.Name,
                PhotoID = mt.PhotoID,
                PhotoData = mt.Photo?.PhotoData
            }));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MealTypeResponseDto>> GetById(string id)
        {
            var mt = await _service.GetById(id);
            if (mt == null) return NotFound();

            return Ok(new MealTypeResponseDto
            {
                MealTypeID = mt.MealTypeID,
                Name = mt.Name,
                PhotoID = mt.PhotoID,
                PhotoData = mt.Photo?.PhotoData
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MealTypeResponseDto>> Create(MealTypeCreateDto dto)
        {
            var entity = new MealTypes
            {
                Name = dto.Name,
                PhotoID = dto.PhotoID
            };

            var created = await _service.Create(entity);
            var photo = await _photoService.GetPhoto(created.PhotoID);

            return CreatedAtAction(nameof(GetById), new { id = created.MealTypeID }, new MealTypeResponseDto
            {
                MealTypeID = created.MealTypeID,
                Name = created.Name,
                PhotoID = created.PhotoID,
                PhotoData = photo?.PhotoData
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<MealTypeResponseDto>> Update(string id, MealTypeCreateDto dto)
        {
            var updated = new MealTypes
            {
                Name = dto.Name,
                PhotoID = dto.PhotoID
            };

            var result = await _service.Update(id, updated);
            if (result == null) return NotFound();

            return Ok(new MealTypeResponseDto
            {
                MealTypeID = result.MealTypeID,
                Name = result.Name,
                PhotoID = result.PhotoID,
                PhotoData = result.Photo?.PhotoData
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _service.Delete(id);
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

