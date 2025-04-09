using HealthyAPI.Data;
using HealthyAPI.DTOs.Photo;
using HealthyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly Context _context;

        public PhotoController(Context context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PhotoResponseDto>> GetById(string id)
        {
            var photo = await _context.Photo.FindAsync(id);
            if (photo == null) return NotFound();

            return Ok(new PhotoResponseDto
            {
                PhotoID = photo.PhotoID,
                PhotoData = photo.PhotoData
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PhotoResponseDto>> Create(PhotoCreateDto dto)
        {
            var photo = new Photo
            {
                PhotoData = dto.PhotoData
            };

            _context.Photo.Add(photo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = photo.PhotoID }, new PhotoResponseDto
            {
                PhotoID = photo.PhotoID,
                PhotoData = photo.PhotoData
            });
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PhotoResponseDto>> Update(string id, PhotoCreateDto dto)
        {
            var photo = await _context.Photo.FindAsync(id);
            if (photo == null) return NotFound();

            photo.PhotoData = dto.PhotoData;
            await _context.SaveChangesAsync();

            return Ok(new PhotoResponseDto
            {
                PhotoID = photo.PhotoID,
                PhotoData = photo.PhotoData
            });
        }
    }
}
