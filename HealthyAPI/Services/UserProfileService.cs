using HealthyAPI.Data;
using HealthyAPI.DTOs.Account;
using HealthyAPI.DTOs.Profile;
using HealthyAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static HealthyAPI.Models.User;

namespace HealthyAPI.Services
{
  public class UserProfileService : IUserProfileService
  {
    private readonly Context _context;
    private readonly JWTservice _jWTService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly EmailService _emailService;
    private readonly IConfiguration _config;

    public UserProfileService(
        Context context,
        JWTservice jWTService,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        EmailService emailService,
        IConfiguration config)
    {
      _context = context;
      _jWTService = jWTService;
      _signInManager = signInManager;
      _userManager = userManager;
      _emailService = emailService;
      _config = config;
    }

    public async Task<UserProfileResponseDto?> GetCurrentUserProfile(string userId)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
        IsFemale = user.IsFemale,
        GoalType = user.GoalType,
        ActivityMultiplier = user.ActivityMultiplier
      };
    }

    public async Task<UserProfileResponseDto?> UpdateProfile(string userId, UpdateUserProfileDto dto)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null) return null;


      user.FirstName = dto.FirstName;
      user.LastName = dto.LastName;
      user.Age = dto.Age;
      user.Height = dto.Height;
      user.BodyFat = dto.BodyFat;
      user.Weight = dto.Weight;
      user.GoalWeight = dto.GoalWeight;
      user.GoalType = dto.GoalType;

      // BMR és TDEE számítás
      double bmr = dto.IsFemale
          ? 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161
          : 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;

      double tdee = bmr * dto.ActivityMultiplier;
      if (dto.GoalType == 1) tdee += 500; // Tömegnövelés
      else if (dto.GoalType == 2) tdee -= 500; // Fogyás
      user.TargetCalorie = (int)tdee;
      user.TargeProtein = user.Weight * 2f;
      user.TargetFat = user.Weight * 1f;
      user.TargetCarb = (float)Math.Round(((tdee - (user.TargeProtein * 4 + user.TargetFat * 9)) / 4));
      user.ActivityMultiplier = dto.ActivityMultiplier;

      await _context.SaveChangesAsync();

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
    public async Task<UserDto?> LoginAsync(LoginDto model)
    {
      var user = await _userManager.FindByNameAsync(model.UserName);
      if (user == null) return null;
      if (user.EmailConfirmed == false) throw new UnauthorizedAccessException("Please confirm your email.");
      var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
      if (!result.Succeeded) return null;
      return CreateApplicationUserDto(user);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterDto model)
    {
      if (await CheckEmailExistsAsync(model.Email))
      { return (false, new[] { $"An existing account is using {model.Email}." });
      }

      var userToAdd = new User
      {
        FirstName = model.FirstName.ToLower(),
        LastName = model.LastName.ToLower(),
        UserName = model.Email.ToLower(),
        Email = model.Email.ToLower()
      };
      var result = await _userManager.CreateAsync(userToAdd, model.Password);
      if (!result.Succeeded)
      {
        var errors = new List<string>();
        foreach (var error in result.Errors) errors.Add(error.Description);
        return (false, errors);
      }

      bool emailSent = await SendConfirmEmailAsync(userToAdd);
      if (!emailSent)
      {
        return (false, new[] { "Failed to send email. Please contact admin." });
      }

      return (true, new string[0]);
    }
    public async Task<UserDto?> RefreshUserTokenAsync(string email)
    {
      var user = await _userManager.FindByNameAsync(email);
      if (user == null) return null;
      return CreateApplicationUserDto(user);
    }
    public async Task<ServiceResult> ConfirmEmailAsync(ConfirmEmailDto model)
    {
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null) return new ServiceResult { Success = false, Message = "This email address has not been registered yet" };
      if (user.EmailConfirmed) return new ServiceResult { Success = false, Message = "Your email was confirmed before. Please login to your account" };

      try
      {
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (result.Succeeded)
        {
          return new ServiceResult { Success = true, Title = "Email confirmed", Message = "Your email address is confirmed. You can login now" };
        }
        return new ServiceResult { Success = false, Message = "Invalid token. Please try again" };
      }
      catch
      {
        return new ServiceResult { Success = false, Message = "Invalid token. Please try again" };
      }
    }

    public async Task<ServiceResult> ResendEmailConfirmationLinkAsync(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null) return new ServiceResult { Success = false, Message = "This email address has not been registered yet" };
      if (user.EmailConfirmed) return new ServiceResult { Success = false, Message = "Your email address was confirmed before. Please login to your account" };

      if (await SendConfirmEmailAsync(user))
      {
        return new ServiceResult { Success = true, Title = "Confirmation link sent", Message = "Please confirm your email address" };
      }
      return new ServiceResult { Success = false, Message = "Failed to send email. Please contact admin" };
    }
    public async Task<ServiceResult> ForgotUsernameOrPasswordAsync(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null) return new ServiceResult { Success = false, Message = "This email address has not been registered yet" };
      if (!user.EmailConfirmed) return new ServiceResult { Success = false, Message = "Please confirm your email address first." };

      if (await SendForgotUsernameOrPasswordEmail(user))
      {
        return new ServiceResult { Success = true, Title = "Forgot username or password email sent", Message = "Please check your email" };
      }
      return new ServiceResult { Success = false, Message = "Failed to send email. Please contact admin" };
    }

    public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto model)
    {
      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null) return new ServiceResult { Success = false, Message = "This email address has not been registered yet" };
      if (!user.EmailConfirmed) return new ServiceResult { Success = false, Message = "Please confirm your email address first" };

      try
      {
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
        if (result.Succeeded)
        {
          return new ServiceResult { Success = true, Title = "Password reset success", Message = "Your password has been reset" };
        }
        return new ServiceResult { Success = false, Message = "Invalid token. Please try again" };
      }
      catch
      {
        return new ServiceResult { Success = false, Message = "Invalid token. Please try again" };
      }
    }
    private UserDto CreateApplicationUserDto(User user)
    {
      return new UserDto
      {
        FirstName = user.FirstName,
        LastName = user.LastName,
        JWT = _jWTService.CreateJWT(user),
      };
    }
    private async Task<bool> CheckEmailExistsAsync(string email)
    {
      return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
    }

    private async Task<bool> SendConfirmEmailAsync(User user)
    {
      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
      var url = $"{_config["JWT:ClientUrl"]}/{_config["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

      var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
          "<p>Please confirm your email address by clicking on the following link.</p>" +
          $"<p><a href=\"{url}\">Click here</a></p>" +
          "<p>Thank you,</p>" +
          $"<br>{_config["Email:ApplicationName"]}";

      var emailSend = new EmailSendDto(user.Email, "Confirm your email", body);
      return await _emailService.SendEmailAsync(emailSend);
    }

    private async Task<bool> SendForgotUsernameOrPasswordEmail(User user)
    {
      var token = await _userManager.GeneratePasswordResetTokenAsync(user);
      token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
      var url = $"{_config["JWT:ClientUrl"]}/{_config["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";

      var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
         $"<p>Username: {user.UserName}.</p>" +
         "<p>In order to reset your password, please click on the following link.</p>" +
         $"<p><a href=\"{url}\">Click here</a></p>" +
         "<p>Thank you,</p>" +
         $"<br>{_config["Email:ApplicationName"]}";

      var emailSend = new EmailSendDto(user.Email, "Forgot username or password", body);
      return await _emailService.SendEmailAsync(emailSend);
    }
  }
}
