using HealthyAPI.DTOs.Account;
using HealthyAPI.Models;
using HealthyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealthyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTservice _jWTService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(JWTservice jWTService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this._jWTService = jWTService;
            this._signInManager = signInManager;
            this._userManager = userManager;
        }
        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);

            return CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Invalid username or password"); // rossz user name
            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email.");
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid username or password");// rossz jelszó 
            return CreateApplicationUserDto(user);

        }
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto model)
        {
            if (await CheckEmailExtistsAsyns(model.Email))
            {
                return BadRequest($"An existing account is using {model.Email}, email addres. Please try with another email address");
            }
            var userToAdd = new User
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true // átmeneti megoldás
            };
            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);


            return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, you can login" }));
        }

        #region Private Helper
        private UserDto CreateApplicationUserDto(User user)// token frssités miatt mindig kell lennie legalább 1 felhasználünak aza datbázisban.
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = _jWTService.CreateJWT(user),

                id = user.Id,// nem szabadna de valahogy meg kell találni az értékeket 
            };
        }
        #endregion
        private async Task<bool> CheckEmailExtistsAsyns(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
