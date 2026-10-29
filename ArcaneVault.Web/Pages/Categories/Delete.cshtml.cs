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
    public class DeleteModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public DeleteModel(IApiClient apiClient) { _apiClient = apiClient; }

        public CategoryViewModel? Category { get; set; }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            Category = await _apiClient.GetCategoryAsync(code);
            if (Category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToPage("Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string code)
        {
            var result = await _apiClient.DeleteCategoryAsync(code);
            TempData[result.Success ? "Success" : "Error"] = result.Success ? "Category deleted!" : result.Message;
            return RedirectToPage("Index");
        }
    }
}
