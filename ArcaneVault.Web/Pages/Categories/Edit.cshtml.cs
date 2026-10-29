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
    public class EditModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public EditModel(IApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty]
        public CategoryCreateViewModel Input { get; set; } = new();

        [BindProperty]
        public string Code { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var cat = await _apiClient.GetCategoryAsync(code);
            if (cat == null) return NotFound();
            Code = code;
            Input.CategoryCode = cat.CategoryCode;
            Input.CategoryName = cat.CategoryName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _apiClient.UpdateCategoryAsync(Code, Input);
            if (result.Success) { TempData["Success"] = "Category updated!"; return RedirectToPage("Index"); }
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }
    }
}
