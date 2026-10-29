// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.Services;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArcaneVault.Web.Pages.Browse
{
    public class IndexModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public IndexModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public BrowseResultViewModel Result { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true, Name = "search")]
        public string Search { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true, Name = "category")]
        public string Category { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true, Name = "sort")]
        public string Sort { get; set; } = "newest";

        [BindProperty(SupportsGet = true, Name = "page")]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true, Name = "minPrice")]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true, Name = "maxPrice")]
        public decimal? MaxPrice { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _apiClient.GetCategoriesAsync();
            var res = await _apiClient.GetBrowseItemsAsync(Search, Category, Sort, PageNumber, 24, MinPrice, MaxPrice);
            if (res != null)
            {
                Result = res;
            }
        }
    }
}
