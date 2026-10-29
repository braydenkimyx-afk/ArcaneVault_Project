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
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public DetailsModel(IApiClient apiClient) { _apiClient = apiClient; }
        public CollectionItemViewModel? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _apiClient.GetItemAsync(id);
            if (Item == null)
            {
                TempData["Error"] = "You don't have permission to edit this item";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
