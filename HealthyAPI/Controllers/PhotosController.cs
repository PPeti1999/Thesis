using HealthyAPI.Data;
using HealthyAPI.DTOs.Photo;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotosController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PhotoResponseDto>>> GetAll()
        {
            var photos = await _photoService.GetAllPhotos();
            return Ok(photos.Select(p => new PhotoResponseDto
            {
                PhotoID = p.PhotoID,
                PhotoData = p.PhotoData
            }));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PhotoResponseDto>> GetById(string id)
        {
            var photo = await _photoService.GetPhoto(id);
            if (photo == null) return NotFound();

            return Ok(new PhotoResponseDto
            {
                PhotoID = photo.PhotoID,
                PhotoData = photo.PhotoData
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PhotoResponseDto>> Upload(PhotoCreateDto dto)
        {
            var entity = new Photo { PhotoData = dto.PhotoData };
            var created = await _photoService.UploadPhoto(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.PhotoID }, new PhotoResponseDto
            {
                PhotoID = created.PhotoID,
                PhotoData = created.PhotoData
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PhotoResponseDto>> Update(string id, PhotoCreateDto dto)
        {
            var updated = await _photoService.UpdatePhoto(id, dto.PhotoData);
            if (updated == null) return NotFound();

            return Ok(new PhotoResponseDto
            {
                PhotoID = updated.PhotoID,
                PhotoData = updated.PhotoData
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _photoService.DeletePhoto(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
