using HealthyAPI.Data;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using HealthyAPI.DTOs.Food;
using System.Linq;

namespace HealthyAPI.Services
{
  public class FoodService : IFoodService
  {
    private readonly Context _context;

    public FoodService(Context context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<IEnumerable<FoodResponseDto>> Search(string query)
    {
      return await _context.Food
          .Where(f => f.Title.ToLower().Contains(query.ToLower()))
          .Select(f => new FoodResponseDto
          {
            FoodID = f.FoodID,
            Title = f.Title,
            Protein = f.Protein,
            Carb = f.Carb,
            Fat = f.Fat,
            Calorie = f.Calorie,
            Gram = f.Gram,
            CreatedAt = f.CreatedAt
          }).ToListAsync();
    }
    public async Task<IEnumerable<FoodResponseDto>> ListFoods()
    {
      var entities = await _context.Food.ToListAsync();
      return entities.Select(f => new FoodResponseDto
      {
        FoodID = f.FoodID,
        Title = f.Title,
        Protein = f.Protein,
        Fat = f.Fat,
        Carb = f.Carb,
        Calorie = f.Calorie,
        Gram = f.Gram,
        CreatedAt = f.CreatedAt
      });
    }

    public async Task<FoodResponseDto> GetFood(string id)
    {
      var entity = await _context.Food.FindAsync(id);
      if (entity == null) return null;
      return new FoodResponseDto
      {
        FoodID = entity.FoodID,
        Title = entity.Title,
        Protein = entity.Protein,
        Fat = entity.Fat,
        Carb = entity.Carb,
        Calorie = entity.Calorie,
        Gram = entity.Gram,
        CreatedAt = entity.CreatedAt
      };
    }

    public async Task<FoodResponseDto> AddFood(FoodCreateDto dto)
    {
      var food = new Food
      {
        Title = dto.Title,
        Protein = dto.Protein,
        Fat = dto.Fat,
        Carb = dto.Carb,
        Calorie = dto.Calorie,
        Gram = dto.Gram,
        CreatedAt = DateTime.UtcNow
      };
      _context.Food.Add(food);
      await _context.SaveChangesAsync();
      return new FoodResponseDto
      {
        FoodID = food.FoodID,
        Title = food.Title,
        Protein = food.Protein,
        Fat = food.Fat,
        Carb = food.Carb,
        Calorie = food.Calorie,
        Gram = food.Gram,
        CreatedAt = food.CreatedAt
      };
    }

    public async Task<FoodResponseDto> UpdateFood(string id, FoodCreateDto dto)
    {
      var existing = await _context.Food.FindAsync(id);
      if (existing == null) return null;
      existing.Title = dto.Title;
      existing.Protein = dto.Protein;
      existing.Fat = dto.Fat;
      existing.Carb = dto.Carb;
      existing.Calorie = dto.Calorie;
      existing.Gram = dto.Gram;
      _context.Food.Update(existing);
      await _context.SaveChangesAsync();
      return new FoodResponseDto
      {
        FoodID = existing.FoodID,
        Title = existing.Title,
        Protein = existing.Protein,
        Fat = existing.Fat,
        Carb = existing.Carb,
        Calorie = existing.Calorie,
        Gram = existing.Gram,
        CreatedAt = existing.CreatedAt
      };
    }

    public async Task<bool> DeleteFood(string id)
    {
      bool hasDependencies = await _context.MealFoods.AnyAsync(dn => dn.FoodID == id);
      if (hasDependencies)
        throw new InvalidOperationException("Has dependencies.");
      var entity = await _context.Food.FindAsync(id);
      if (entity == null) return false;
      _context.Food.Remove(entity);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}

