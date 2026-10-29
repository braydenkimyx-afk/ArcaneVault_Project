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
    public class CreateModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public CreateModel(IApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty]
        public CollectionItemCreateViewModel Input { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public List<CategoryViewModel> AvailableCategories { get; set; } = new();

        public async Task OnGetAsync()
        {
            AvailableCategories = await _apiClient.GetCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableCategories = await _apiClient.GetCategoriesAsync();
                return Page();
            }

            if (ImageFile != null)
            {
                if (ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Image size exceeds 2MB limit.");
                }
                else if (ImageFile.ContentType != "image/jpeg" && ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Only JPEG and PNG images are supported.");
                }
                else
                {
                    using var ms = new MemoryStream();
                    await ImageFile.CopyToAsync(ms);
                    Input.ImageBase64 = Convert.ToBase64String(ms.ToArray());
                    Input.ImageContentType = ImageFile.ContentType;
                }
            }

            if (!ModelState.IsValid)
            {
                AvailableCategories = await _apiClient.GetCategoriesAsync();
                return Page();
            }

            var result = await _apiClient.CreateItemAsync(Input);
            if (result.Success) { TempData["Success"] = "Item added!"; return RedirectToPage("Index"); }
            ModelState.AddModelError(string.Empty, result.Message);
            AvailableCategories = await _apiClient.GetCategoriesAsync();
            return Page();
        }
    }
}
