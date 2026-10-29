// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.Security.Claims;
using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public LoginModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _apiClient.LoginAsync(Input);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Data!.UserName),
                new(ClaimTypes.Role, result.Data.RoleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2) });

            TempData["Success"] = $"Welcome back, {result.Data.UserName}!";
            return RedirectToPage("/Index");
        }
    }
}
