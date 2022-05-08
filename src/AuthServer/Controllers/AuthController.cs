using AuthServer.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthServer.Controllers
{
    public class AuthController : Controller
    {

        /// <summary>
        /// Serves the login page and optionally stores the returnUrl
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            
            ViewData["ReturnUrl"] = model.ReturnUrl;

            if (ModelState.IsValid) 
            {

                //todo here: check credentials

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, model.Username)
                };

                //authenticationType matches the one defined in Program AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                var claimsIdentity = new ClaimsIdentity(claims, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                //calls the AuthenticationService which calls the CookieAuthenticationHandler because that's the scheme we specified when creating the claims identity
                await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                //to protect against open redirect attacks
                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            //calls the cookie authentication middleware, to sign out the user.
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }   
    }
}
