// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public ResetPasswordModel(IApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty]
        public ResetPasswordViewModel Input { get; set; } = new();

        public void OnGet(string token, string userName)
        {
            Input.Token = token;
            Input.UserName = userName;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _apiClient.ResetPasswordAsync(Input);
            if (result.Success)
            {
                TempData["Success"] = "Password reset successfully! Please login.";
                return RedirectToPage("/Account/Login");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }
    }
}
