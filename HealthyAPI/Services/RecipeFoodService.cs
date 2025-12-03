using HealthyAPI.Data;
using HealthyAPI.DTOs.RecipeFood;
using HealthyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public class RecipeFoodService : IRecipeFoodService
    {
        private readonly Context _context;

        public RecipeFoodService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecipeFoodResponseDto>> GetAll()
        {
          var entities = await _context.RecipeFoods
            .Include(rf => rf.Food)
            .ToListAsync();
          return entities.Select(rf => new RecipeFoodResponseDto
          {
            RecipeFoodID = rf.RecipeFoodID,
            RecipeID = rf.RecipeID,
            FoodID = rf.FoodID,
            FoodName = rf.Food?.Title,
            Quantity = rf.Quantity
          });
        }
        public async Task<RecipeFoodResponseDto?> GetById(string id)
        {
          var rf = await _context.RecipeFoods
              .Include(rf => rf.Food)
              .FirstOrDefaultAsync(x => x.RecipeFoodID == id);
          if (rf == null) return null;
          return new RecipeFoodResponseDto
          {
            RecipeFoodID = rf.RecipeFoodID,
            RecipeID = rf.RecipeID,
            FoodID = rf.FoodID,
            FoodName = rf.Food?.Title,
            Quantity = rf.Quantity
          };
        }

        public async Task<RecipeFoodResponseDto> Create(RecipeFoodCreateDto dto)
        {
          var entity = new RecipeFoods
          {
            RecipeFoodID = Guid.NewGuid().ToString(),
            RecipeID = dto.RecipeID,
            FoodID = dto.FoodID,
            Quantity = dto.Quantity
          };
          _context.RecipeFoods.Add(entity);
          await _context.SaveChangesAsync();
          var createdEntity = await _context.RecipeFoods
              .Include(rf => rf.Food)
              .FirstOrDefaultAsync(rf => rf.RecipeFoodID == entity.RecipeFoodID);
          return new RecipeFoodResponseDto
          {
            RecipeFoodID = createdEntity.RecipeFoodID,
            RecipeID = createdEntity.RecipeID,
            FoodID = createdEntity.FoodID,
            FoodName = createdEntity.Food?.Title,
            Quantity = createdEntity.Quantity
          };
        }

        public async Task<RecipeFoodResponseDto?> Update(string id, RecipeFoodCreateDto dto)
        {
          var entity = await _context.RecipeFoods.FindAsync(id);
          if (entity == null) return null;
          entity.FoodID = dto.FoodID;
          entity.RecipeID = dto.RecipeID;
          entity.Quantity = dto.Quantity;
          await _context.SaveChangesAsync();
          var updatedEntity = await _context.RecipeFoods
              .Include(rf => rf.Food)
              .FirstOrDefaultAsync(rf => rf.RecipeFoodID == id);
          return new RecipeFoodResponseDto
          {
            RecipeFoodID = updatedEntity.RecipeFoodID,
            RecipeID = updatedEntity.RecipeID,
            FoodID = updatedEntity.FoodID,
            FoodName = updatedEntity.Food?.Title,
            Quantity = updatedEntity.Quantity
          };
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _context.RecipeFoods.FindAsync(id);
            if (entity == null) return false;

            _context.RecipeFoods.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
