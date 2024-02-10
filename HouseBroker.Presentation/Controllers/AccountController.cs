using HouseBroker.Application.Dtos.Auth;
using HouseBroker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HouseBroker.Presentation.Controllers
{
    public class AccountController : Controller
    {
        #region Properties
        private readonly IAuthService _authenticationService;
        #endregion

        #region Ctor
        public AccountController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// register page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Register()
        {
            var model = new RegisterDto();
            model.roles = await _authenticationService.RolesListAsync();
            return View(model);
        }

        /// <summary>
        /// create a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.RegisterAsync(model);

                if (result.IsSucceed)
                {
                    return RedirectToAction("Login", "Account");
                }
                return View(model);
            }
            return View(model);
        }

        /// <summary>
        /// login view
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// login the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto user, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.LoginAsync(user);
                if (result.UserInfo != null)
                {
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(user);
            }
            return View(user);
        }

        /// <summary>
        /// redirect to local
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        /// <summary>
        /// logout user
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.SignOutAsync();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// AccessDenie page
        /// </summary>
        /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
    }
}
