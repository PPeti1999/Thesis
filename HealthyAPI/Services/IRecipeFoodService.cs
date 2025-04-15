using HealthyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public interface IRecipeFoodService
    {
        Task<IEnumerable<RecipeFoods>> GetAll();
        Task<RecipeFoods?> GetById(string id);
        Task<RecipeFoods> Create(RecipeFoods entity);
        Task<RecipeFoods?> Update(string id, RecipeFoods updated);
        Task<bool> Delete(string id);
    }
}
