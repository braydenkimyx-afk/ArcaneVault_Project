// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.CollectionItems
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public DeleteModel(IApiClient apiClient) { _apiClient = apiClient; }

        public CollectionItemViewModel? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _apiClient.GetItemAsync(id);
            if (item == null || (item.UserName != User.Identity?.Name && !User.IsInRole("Staff")))
            {
                TempData["Error"] = "You don't have permission to delete this item";
                return RedirectToPage("Index");
            }

            Item = item;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var result = await _apiClient.DeleteItemAsync(id);
            if (!result.Success && result.Message != null && result.Message.Contains("Forbidden"))
            {
                TempData["Error"] = "You don't have permission to edit this item";
            }
            else
            {
                TempData[result.Success ? "Success" : "Error"] = result.Success ? "Item deleted!" : result.Message;
            }
            return RedirectToPage("Index");
        }
    }
}
