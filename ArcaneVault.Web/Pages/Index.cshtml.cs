// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public IndexModel(IApiClient apiClient) { _apiClient = apiClient; }

        public DashboardViewModel? DashboardStats { get; set; }
        public int TotalUsers { get; set; } = 0;
        public List<CollectionItemViewModel> RecentItems { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // Fetch public platform-wide stats by passing null for user and role
                DashboardStats = await _apiClient.GetDashboardAsync(null, null);

                // Try fetching users (may be access restricted depending on API, wrapping in try/catch)
                var users = await _apiClient.GetUsersAsync();
                TotalUsers = users?.Count ?? 0;
            }
            catch
            {
                // Graceful fallback if endpoints are protected or fail
                if (DashboardStats == null)
                {
                    DashboardStats = new DashboardViewModel { TotalItems = 0, CategoriesUsed = 0 };
                }
            }
            
            try
            {
                // Fetch public recent items showcase (Unscoped)
                RecentItems = await _apiClient.GetRecentItemsAsync(10);
            }
            catch
            {
                RecentItems = new();
            }
        }
    }
}
