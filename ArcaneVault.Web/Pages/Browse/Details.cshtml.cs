// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Browse
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public DetailsModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public BrowseItemViewModel Item { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _apiClient.GetBrowseItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            Item = item;
            return Page();
        }
    }
}
