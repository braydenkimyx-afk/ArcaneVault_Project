// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public RegisterModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _apiClient.RegisterAsync(Input);
            if (result.Success)
            {
                TempData["Success"] = "Account created successfully! Please login.";
                return RedirectToPage("/Account/Login");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return Page();
        }

        public async Task<JsonResult> OnGetCheckEmailAsync(string email)
        {
            var available = await _apiClient.CheckEmailAvailableAsync(email);
            return new JsonResult(available);
        }
    }
}
