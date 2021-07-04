using Authorize.Data;
using Authorize.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorize.Entities;
using Microsoft.AspNetCore.Identity;

namespace Authorize.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="Mentor")]
        public IActionResult Mentor()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Student()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                Providers = externalProviders
            };
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ExternalSigIn(string returnUrl, string provider)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallBack), "Admin", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl)
        {
            var info =await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            var user = new ApplicationUser()
            {
                UserName = "ArtemFacebook"
            };

            var resultSec = await _userManager.CreateAsync(user);

            if (resultSec.Succeeded)
            {
                var claims = new List<Claim>
                    {new Claim(ClaimTypes.Role, "Mentor"), new Claim(ClaimTypes.Role, "Student")};
                var claimsResult = await _userManager.AddClaimsAsync(user, claims);
                if (claimsResult.Succeeded)
                {
                    var identityResult = await _userManager.AddLoginAsync(user, info);

                    if (identityResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index");
                    }
                }
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            var result =await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(model);

        }

        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Redirect("https://localhost:44384/Home/Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterExternal(ExternalLoginViewModel externalLoginViewModel)
        {
            return RedirectToAction("",externalLoginViewModel);
        }

        [AllowAnonymous]
        [ActionName("RegisterExternal")]
        [HttpPost]
        public async Task<IActionResult> RegisterExternalConfirmed(ExternalLoginViewModel model)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                RedirectToAction("Login");
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                    {new Claim(ClaimTypes.Role, "Mentor"), new Claim(ClaimTypes.Role, "Student")};
                var claimsResult =await _userManager.AddClaimsAsync(user, claims);
                if (claimsResult.Succeeded)
                {
                    var identityResult = await _userManager.AddLoginAsync(user, info);

                    if (identityResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index");
                    }
                }
            }

            return View(model);
        }
    }

    public class ExternalLoginViewModel
    {
        public string ReturnUrl { get; set; }

        public string UserName { get; set; }
    }
}
