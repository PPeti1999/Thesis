using HealthyAPI.Data;
using HealthyAPI.DTOs.Profile;
using Microsoft.EntityFrameworkCore;
using System;
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
                GoalType = user.GoalType,
                ActivityMultiplier = user.ActivityMultiplier


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

            // 🔢 BMR + TDEE
            double bmr = dto.IsFemale
                ? 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161
                : 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;

            double tdee = bmr * dto.ActivityMultiplier;

            // 🎯 Cél módosítás (tömegelés / fogyás)
            if (dto.GoalType == 1) tdee += 500;
            else if (dto.GoalType == 2) tdee -= 500;

            user.TargetCalorie = (int)tdee;
            user.TargeProtein = user.Weight * 2f;
            user.TargetFat = user.Weight * 1f;
            user.TargetCarb = (float)((tdee - (user.TargeProtein * 4 + user.TargetFat * 9)) / 4);
            user.ActivityMultiplier = dto.ActivityMultiplier;

            await _context.SaveChangesAsync();

            // 🔁 Frissítjük a mai DailyNote célértékeit is, ha létezik
            var todayNote = await _context.DailyNote
                .FirstOrDefaultAsync(d => d.UserID == user.Id && d.CreatedAt.Date == DateTime.Today);

            if (todayNote != null)
            {
                todayNote.DailyTargetCalorie = user.TargetCalorie;
                todayNote.DailyTargetProtein = user.TargeProtein;
                todayNote.DailyTargetCarb = user.TargetCarb;
                todayNote.DailyTargetFat = user.TargetFat;
                await _context.SaveChangesAsync();
            }

            return await GetCurrentUserProfile(userId);
        }
    }
}
