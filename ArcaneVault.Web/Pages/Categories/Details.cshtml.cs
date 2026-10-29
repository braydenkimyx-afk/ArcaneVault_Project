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
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public DetailsModel(IApiClient apiClient) { _apiClient = apiClient; }
        public CategoryViewModel? Category { get; set; }

        public async Task OnGetAsync(string code)
        {
            Category = await _apiClient.GetCategoryAsync(code);
        }
    }
}
