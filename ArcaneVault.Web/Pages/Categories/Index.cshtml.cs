// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Categories
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public IndexModel(IApiClient apiClient) { _apiClient = apiClient; }
        public List<CategoryViewModel> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _apiClient.GetCategoriesAsync();
        }
    }
}
