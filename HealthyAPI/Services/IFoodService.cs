using HealthyAPI.DTOs.Food;
using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{

    public interface IFoodService
    {
        Task<IEnumerable<FoodResponseDto>> ListFoods();
        Task<FoodResponseDto> GetFood(string id);
        Task<FoodResponseDto> AddFood(FoodCreateDto food);
        Task<FoodResponseDto> UpdateFood(string id, FoodCreateDto food);
        Task<bool> DeleteFood(string id);
        Task<IEnumerable<FoodResponseDto>> Search(string query);

    }

}
