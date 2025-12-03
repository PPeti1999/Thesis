using HealthyAPI.Data;
using HealthyAPI.DTOs.MealType;
using HealthyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
  public class MealTypeService : IMealTypeService
  {
    private readonly Context _context;

    public MealTypeService(Context context)
    {
      _context = context;
    }

    public async Task<IEnumerable<MealTypeResponseDto>> GetAll()
    {
      var entities = await _context.MealTypes.ToListAsync();
      return entities.Select(mt => new MealTypeResponseDto
      {
        MealTypeID = mt.MealTypeID,
        Name = mt.Name,

      });
    }

    public async Task<MealTypeResponseDto> GetById(string id)
    {
      var mt = await _context.MealTypes
          .FirstOrDefaultAsync(mt => mt.MealTypeID == id);
      if (mt == null) return null;
      return new MealTypeResponseDto
      {
        MealTypeID = mt.MealTypeID,
        Name = mt.Name
      };
    }

    public async Task<MealTypeResponseDto> Create(MealTypeCreateDto created)
    {
      var entity = new MealTypes
      {
        Name = created.Name
      };
      _context.MealTypes.Add(entity);
      await _context.SaveChangesAsync();
      return new MealTypeResponseDto
      {
        MealTypeID = entity.MealTypeID,
        Name = entity.Name
      };
    }

    public async Task<MealTypeResponseDto> Update(string id, MealTypeCreateDto updated)
    {
      var entity = await _context.MealTypes.FindAsync(id);
      if (entity == null) return null;
      entity.Name = updated.Name;
      _context.MealTypes.Update(entity);
      await _context.SaveChangesAsync();
      return new MealTypeResponseDto
      {
        MealTypeID = entity.MealTypeID,
        Name = entity.Name
      };
    }

    public async Task<bool> Delete(string id)
    {
      var entity = await _context.MealTypes.FindAsync(id);
      if (entity == null) return false;
      bool hasEntries = await _context.MealEntries.AnyAsync(me => me.MealTypeID == id);
      if (hasEntries)
        throw new InvalidOperationException("Has Dependency");
      _context.MealTypes.Remove(entity);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}
