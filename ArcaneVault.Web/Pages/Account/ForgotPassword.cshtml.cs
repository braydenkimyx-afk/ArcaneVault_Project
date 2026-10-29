// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public ForgotPasswordModel(IApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty]
        public ForgotPasswordViewModel Input { get; set; } = new();
        public string? ResetToken { get; set; }
        public string? ResetUserName { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _apiClient.RequestPasswordResetAsync(Input.Email);
            if (result.Success && result.Data != null)
            {
                ResetToken = result.Data.Token;
                ResetUserName = result.Data.UserName;
            }
            else
            {
                TempData["Success"] = "If that email is registered, a reset link has been generated.";
            }
            return Page();
        }
    }
}
