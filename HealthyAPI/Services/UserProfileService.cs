using HealthyAPI.Data;
using HealthyAPI.DTOs.Profile;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly Context _context;

        public UserProfileService(Context context)
        {
            _context = context;
        }

        public async Task<UserProfileResponseDto?> GetCurrentUserProfile(string userId)
        {
            var user = await _context.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            return new UserProfileResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Height = user.Height,
                BodyFat = user.BodyFat,
                Weight = user.Weight,
                GoalWeight = user.GoalWeight,
                TargetCalorie = user.TargetCalorie,
                TargetProtein = user.TargeProtein,
                TargetCarb = user.TargetCarb,
                TargetFat = user.TargetFat,
                PhotoID = user.PhotoID,
                PhotoData = user.Photo?.PhotoData,
                IsFemale = user.IsFemale,
                GoalType = user.GoalType

            };
        }

        public async Task<UserProfileResponseDto?> UpdateProfile(string userId, UpdateUserProfileDto dto)
        {
            var user = await _context.Users.Include(u => u.Photo).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Age = dto.Age;
            user.Height = dto.Height;
            user.BodyFat = dto.BodyFat;
            user.Weight = dto.Weight;
            user.GoalWeight = dto.GoalWeight;
            user.GoalType = dto.GoalType;
            // 🔢 Mifflin–St Jeor BMR képlet nemenként
            double bmr = dto.IsFemale
                ? 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161
                : 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;

            double tdee = bmr * dto.ActivityMultiplier;

            // 🎯 Cél alapján módosítás (tömegelés, fogyás, megtartás)
            if (dto.GoalType == 1) tdee += 500;      // tömegelés
            else if (dto.GoalType == 2) tdee -= 500; // diéta

            user.TargetCalorie = (int)tdee;
            user.TargeProtein = user.Weight * 2f;
            user.TargetFat = user.Weight * 1f;
            user.TargetCarb = (float)((tdee - (user.TargeProtein * 4 + user.TargetFat * 9)) / 4);

            await _context.SaveChangesAsync();

            return await GetCurrentUserProfile(userId);
            /*Aktivitási szint	Szorzó
            Nagyon alacsony (pl. irodai munka)	1.2
            Enyhén aktív (heti 1–3 edzés)	1.375
            Mérsékelten aktív (heti 3–5 edzés)	1.55
            Nagyon aktív (heti 6–7 edzés)	1.725
            Extrém aktív (napi edzés, fizikai munka)	1.9*/
        }
    }
}
