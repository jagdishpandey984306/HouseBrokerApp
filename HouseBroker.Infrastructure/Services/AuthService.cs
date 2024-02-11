using HouseBroker.Application.Dtos.Auth;
using HouseBroker.Application.Dtos.General;
using HouseBroker.Application.Interfaces;
using HouseBroker.Domain.Constants;
using HouseBroker.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseBroker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        #region Properties
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<AuthService> _log;
        #endregion

        #region Ctor
        public AuthService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContext,
            ILogger<AuthService> log)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _httpContext = httpContext;
            _log = log;
        }
        #endregion

        #region Methods

        /// <summary>
        /// find the user by username
        /// if exists return null
        /// check password and if incorrect return null
        /// generate token,get user role and return user details after login successfully
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Find user with username
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null)
                _log.LogInformation("user is null");
                return null;

            // check password of user
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return null;

            // Return Token and userInfo to front-end
            var newToken = await GenerateJWTTokenAsync(user);
            _log.LogInformation($"token generated{newToken}");
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);

            return new LoginResponseDto()
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }

        /// <summary>
        /// find the user by username
        /// check user if exists or not 
        /// create user if user is not exists and if exists return user
        /// add role with userid in userroles table
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser is not null)
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "UserName Already Exists"
                };

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Address = registerDto.Address,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation failed because: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = errorString
                };
            }

            await _userManager.AddToRoleAsync(newUser, registerDto.Role);

            return new GeneralServiceResponseDto()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "User Created Successfully"
            };
        }

        /// <summary>
        /// get role list
        /// </summary>
        /// <returns></returns>
        public async Task<IList<SelectListItem>> RolesListAsync()
        {
            var data = await _roleManager.Roles.ToListAsync();
            var roles = data.Select(p => new SelectListItem()
            {
                Value = p.Name,
                Text = p.Name
            }).ToList();
            return roles;
        }

        /// <summary>
        /// this service is used to seed roles  directly in database
        /// check roles exists,if not exists saved roles into roles table 
        /// </summary>
        /// <returns></returns>
        public async Task<GeneralServiceResponseDto> SeedRolesAsync()
        {
            bool isHouseSeekerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.HOUSESEEKER);
            bool isBrokerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.BROKER);

            if (isHouseSeekerRoleExists && isBrokerRoleExists)
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.HOUSESEEKER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.BROKER));

            return new GeneralServiceResponseDto()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Roles Seeding Done Successfully"
            };
        }

        /// <summary>
        /// logout the user
        /// </summary>
        /// <returns></returns>

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        #endregion

        #region Utilities
        /// <summary>
        /// generate token,claims and return token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("UserId", user.Id)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            identity.AddClaims(authClaims);
            await _httpContext.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

       /// <summary>
       /// return user details
       /// </summary>
       /// <param name="user"></param>
       /// <param name="Roles"></param>
       /// <returns></returns>
        private UserInfoResult GenerateUserInfoObject(ApplicationUser user, IEnumerable<string> Roles)
        {
            return new UserInfoResult()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = Roles
            };
        }
        #endregion
    }
}
