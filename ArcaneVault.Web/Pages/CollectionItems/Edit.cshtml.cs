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
    public class EditModel : PageModel
    {
        private readonly IApiClient _apiClient;
        public EditModel(IApiClient apiClient) { _apiClient = apiClient; }

        [BindProperty]
        public CollectionItemUpdateViewModel Input { get; set; } = new();

        [BindProperty]
        public int ItemId { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public bool HasImage { get; set; }

        public List<CategoryViewModel> AvailableCategories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _apiClient.GetItemAsync(id);
            if (item == null || item.UserName != User.Identity?.Name)
            {
                TempData["Error"] = "You don't have permission to edit this item";
                return RedirectToPage("Index");
            }

            ItemId = id;
            Input.ItemName = item.ItemName;
            Input.StartingQuantity = item.StartingQuantity;
            Input.CurrentQuantity = item.CurrentQuantity;
            Input.Price = item.Price;
            Input.CategoryCodes = item.Categories.Select(c => c.CategoryCode).ToList();
            HasImage = item.HasImage;
            AvailableCategories = await _apiClient.GetCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableCategories = await _apiClient.GetCategoriesAsync();
                var currentItem = await _apiClient.GetItemAsync(ItemId);
                if (currentItem != null) HasImage = currentItem.HasImage;
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
                var currentItem = await _apiClient.GetItemAsync(ItemId);
                if (currentItem != null) HasImage = currentItem.HasImage;
                return Page();
            }

            var result = await _apiClient.UpdateItemAsync(ItemId, Input);
            if (result.Success) { TempData["Success"] = "Item updated!"; return RedirectToPage("Index"); }

            if (result.Message != null && result.Message.Contains("Forbidden"))
            {
                TempData["Error"] = "You don't have permission to edit this item";
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "An error occurred.");
            AvailableCategories = await _apiClient.GetCategoriesAsync();
            return Page();
        }
    }
}
