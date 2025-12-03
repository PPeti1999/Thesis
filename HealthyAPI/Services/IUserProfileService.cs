using HealthyAPI.DTOs.Account;
using HealthyAPI.DTOs.Profile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyAPI.Services
{
  public interface IUserProfileService
  {
    Task<UserProfileResponseDto> GetCurrentUserProfile(string userId);
    Task<UserProfileResponseDto> UpdateProfile(string userId, UpdateUserProfileDto dto);

    Task<UserDto?> LoginAsync(LoginDto model);
    Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterDto model);
    Task<UserDto?> RefreshUserTokenAsync(string email);
    Task<ServiceResult> ConfirmEmailAsync(ConfirmEmailDto model);
    Task<ServiceResult> ResendEmailConfirmationLinkAsync(string email);
    Task<ServiceResult> ForgotUsernameOrPasswordAsync(string email);
    Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto model);
  }

  public class ServiceResult
  {
    public bool Success { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
  }
}
