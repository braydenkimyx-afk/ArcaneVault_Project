// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVault.Web.ViewModels;

namespace ArcaneVault.Web.Services
{
    /// <summary>
    /// Typed HTTP client interface for all API communication.
    /// The Web project NEVER touches the database directly — all data access goes through these methods.
    /// </summary>
    public interface IApiClient
    {
        // Account
        Task<List<UserViewModel>> GetUsersAsync();
        Task<UserViewModel?> GetUserAsync(string userName);
        Task<ApiResult<UserViewModel>> RegisterAsync(RegisterViewModel model);
        Task<ApiResult<LoginResponseViewModel>> LoginAsync(LoginViewModel model);
        Task<ApiResult> UpdateUserAsync(string userName, UserUpdateViewModel model);
        Task<ApiResult> DeleteUserAsync(string userName);
        Task<bool> CheckEmailAvailableAsync(string email, string? currentUserName = null);
        Task<ApiResult<PasswordResetTokenResult>> RequestPasswordResetAsync(string email);
        Task<ApiResult> ResetPasswordAsync(ResetPasswordViewModel model);

        // Categories
        Task<List<CategoryViewModel>> GetCategoriesAsync();
        Task<CategoryViewModel?> GetCategoryAsync(string categoryCode);
        Task<ApiResult<CategoryViewModel>> CreateCategoryAsync(CategoryCreateViewModel model);
        Task<ApiResult> UpdateCategoryAsync(string categoryCode, CategoryCreateViewModel model);
        Task<ApiResult> DeleteCategoryAsync(string categoryCode);

        // Collection Items
        Task<List<CollectionItemViewModel>> GetItemsAsync();
        Task<CollectionItemViewModel?> GetItemAsync(int id);
        Task<ApiResult<CollectionItemViewModel>> CreateItemAsync(CollectionItemCreateViewModel model);
        Task<ApiResult> UpdateItemAsync(int id, CollectionItemUpdateViewModel model);
        Task<ApiResult> DeleteItemAsync(int id);
        Task<List<CollectionItemViewModel>> SearchItemsAsync(string query);
        Task<List<CollectionItemViewModel>> GetRecentItemsAsync(int take = 10);
        Task<BrowseResultViewModel?> GetBrowseItemsAsync(string search = "", string category = "", string sort = "newest", int page = 1, int pageSize = 24, decimal? minPrice = null, decimal? maxPrice = null);
        Task<BrowseItemViewModel?> GetBrowseItemAsync(int id);

        // Analytics
        Task<DashboardViewModel?> GetDashboardAsync(string? userName = null, string? role = null);
        Task<List<LowStockItemViewModel>> GetLowStockAsync(string? userName = null, string? role = null);
        Task<byte[]> ExportCsvAsync(string? userName = null, string? role = null);
        Task<CompletionViewModel?> GetCompletionAsync(string? userName = null, string? role = null);
    }

    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }
    }

    public class PasswordResetTokenResult
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
