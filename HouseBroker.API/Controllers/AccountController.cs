using HouseBroker.Application.Dtos.Auth;
using HouseBroker.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Properties
        private readonly IAuthService _authService;
        #endregion

        #region Ctor
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion

        #region Methods

        // Route -> Seed Roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedResult = await _authService.SeedRolesAsync();
            return StatusCode(seedResult.StatusCode, seedResult.Message);
        }

        // Route -> return  list of roles
        [HttpGet]
        [Route("role-list")]
        public async Task<IActionResult> RolesList()
        {
            var seedResult = await _authService.RolesListAsync();
            return Ok(seedResult);
        }

        // Route -> Register the user
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResult = await _authService.RegisterAsync(registerDto);
            return StatusCode(registerResult.StatusCode, registerResult.Message);
        }

        // Route -> aunticate the user
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);
            if (loginResult is null)
            {
                return Unauthorized("Your credentials are invalid. Please contact to an Admin");
            }
            return Ok(loginResult);
        }
        #endregion
    }
}
