using HealthyAPI.Data;
using HealthyAPI.DTOs.ActivityCatalog;
using HealthyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using HealthyAPI.Services;
using HealthyAPI.DTOs.Food;

namespace HealthyAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ActivityCatalogController : ControllerBase
  {
    private readonly IActivityCatalogService _service;

    public ActivityCatalogController(IActivityCatalogService service)
    {
      _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityCatalogResponseDto>>> GetAll()
    {
      var dtos = await _service.GetAll();
      return Ok(dtos);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ActivityCatalogResponseDto>> GetById(string id)
    {
      var dto = await _service.GetById(id);
      if (dto == null) return NotFound();
      return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ActivityCatalogResponseDto>> Create([FromBody] ActivityCatalogCreateDto dto)
    {
      var createdDto = await _service.Create(dto);
      return Ok(createdDto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ActivityCatalogResponseDto>> Update(string id, [FromBody] ActivityCatalogCreateDto dto)
    {
      var updatedDto = await _service.Update(id, dto);
      if (updatedDto == null) return NotFound();
      return Ok(updatedDto);
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
