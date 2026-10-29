// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.Security.Claims;
using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.CollectionItems
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public IndexModel(IApiClient apiClient) { _apiClient = apiClient; }
        public List<CollectionItemViewModel> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public async Task OnGetAsync([FromQuery(Name = "q")] string? q)
        {
            SearchQuery = q;
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var isStaff = User.IsInRole("Staff");

            if (!string.IsNullOrWhiteSpace(q))
                Items = await _apiClient.SearchItemsAsync(q);
            else
                Items = await _apiClient.GetItemsAsync();
        }
    }
}
