using HealthyAPI.Data;
using HealthyAPI.DTOs.Account;
using HealthyAPI.DTOs.Profile;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly IUserProfileService _userProfileService;

    public AccountController(IUserProfileService userProfileService)
    {
      _userProfileService = userProfileService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileResponseDto>> GetProfile()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var profile = await _userProfileService.GetCurrentUserProfile(userId);
      if (profile == null) return NotFound();
      return Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileResponseDto>> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var updated = await _userProfileService.UpdateProfile(userId, dto);
      if (updated == null) return NotFound();
      return Ok(updated);
    }
    [Authorize]
    [HttpGet("refresh-user-token")]
    public async Task<ActionResult<UserDto>> RefreshUserToken()
    {
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      var userDto = await _userProfileService.RefreshUserTokenAsync(email);
      if (userDto == null) return Unauthorized();
      return Ok(userDto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto model)
    {
      try
      {
        var userDto = await _userProfileService.LoginAsync(model);
        if (userDto == null) return Unauthorized("Invalid username or password");
        return Ok(userDto);
      }
      catch (UnauthorizedAccessException ex)
      {
        return Unauthorized(ex.Message);
      }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto model)
    {
      var (succeeded, errors) = await _userProfileService.RegisterAsync(model);

      if (succeeded)
      {return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, please confirm your email address" }));
      }

      return BadRequest(errors);
    }

    [HttpPut("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
    {
      var result = await _userProfileService.ConfirmEmailAsync(model);
      if (result.Success)
      { return Ok(new JsonResult(new { title = result.Title, message = result.Message }));
      }
      return BadRequest(result.Message);
    }

    [HttpPost("resend-email-confirmation-link/{email}")]
    public async Task<IActionResult> ResendEMailConfirmationLink(string email)
    {
      if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");

      var result = await _userProfileService.ResendEmailConfirmationLinkAsync(email);
      if (result.Success)
      {
        return Ok(new JsonResult(new { title = result.Title, message = result.Message }));
      }
      return BadRequest(result.Message);
    }
    [HttpPost("forgot-username-or-password/{email}")]
    public async Task<IActionResult> ForgotUsernameOrPassword(string email)
    {
      if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");

      var result = await _userProfileService.ForgotUsernameOrPasswordAsync(email);
      if (result.Success)
      {return Ok(new JsonResult(new { title = result.Title, message = result.Message }));
      }
      return BadRequest(result.Message);
    }
    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
      var result = await _userProfileService.ResetPasswordAsync(model);
      if (result.Success)
      {
        return Ok(new JsonResult(new { title = result.Title, message = result.Message }));
      }
      return BadRequest(result.Message);
    }
  }
}
