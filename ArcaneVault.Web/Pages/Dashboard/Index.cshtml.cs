// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.Security.Claims;
using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public IndexModel(IApiClient apiClient) { _apiClient = apiClient; }

        public DashboardViewModel? Dashboard { get; set; }
        public CompletionViewModel? Completion { get; set; }

        public async Task OnGetAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            Dashboard = await _apiClient.GetDashboardAsync(userName, role);
            Completion = await _apiClient.GetCompletionAsync(userName, role);
        }

        public async Task<IActionResult> OnGetExportCsvAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var csv = await _apiClient.ExportCsvAsync(userName, role);
            return File(csv, "text/csv", "collection_export.csv");
        }
    }
}
