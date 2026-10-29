// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Categories
{
    [Authorize(Roles = "Staff")]
    public class CreateModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public CreateModel(IApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty]
        public CategoryCreateViewModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _apiClient.CreateCategoryAsync(Input);
            if (result.Success) { TempData["Success"] = "Category created!"; return RedirectToPage("Index"); }
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }
    }
}
