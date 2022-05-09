using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServer.BlazorServer.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } 
        
        [BindProperty]
        [DataType((DataType.Password))]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        //this is called by convention when the submit button is clicked in the page form
        public async Task<IActionResult> OnPostAsync()
        {
            if (!(Email == "1" && Password == "1"))
            {
                //todo check password
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, Email),
                new(ClaimTypes.Name, "Bob Freever")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            //there should be a coookie now, so redirect to the root
            return LocalRedirect("~/");
        }
    }
}
