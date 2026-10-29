// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using ArcaneVault.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ArcaneVault.Web.Services
{
    /// <summary>
    /// Typed HttpClient implementation for all API communication.
    /// All data access flows through this service — the Web project never touches the database.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            if (_configuration["ApiSettings:ApiKey"] != null)
            {
                _http.DefaultRequestHeaders.Remove("X-Api-Key");
                _http.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", _configuration["ApiSettings:ApiKey"]);
            }

            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userName = user.FindFirstValue(ClaimTypes.Name);
                if (!string.IsNullOrEmpty(userName)) 
                {
                    _http.DefaultRequestHeaders.Remove("X-User-Name");
                    _http.DefaultRequestHeaders.TryAddWithoutValidation("X-User-Name", userName);
                }

                var role = user.FindFirstValue(ClaimTypes.Role);
                if (!string.IsNullOrEmpty(role)) 
                {
                    _http.DefaultRequestHeaders.Remove("X-User-Role");
                    _http.DefaultRequestHeaders.TryAddWithoutValidation("X-User-Role", role);
                }
            }
        }

        // ===== Account =====

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            var response = await _http.GetAsync("/api/account");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<UserViewModel>>(_jsonOptions) ?? new();
            return new();
        }

        public async Task<UserViewModel?> GetUserAsync(string userName)
        {
            var response = await _http.GetAsync($"/api/account/{userName}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UserViewModel>(_jsonOptions);
            return null;
        }

        public async Task<ApiResult<UserViewModel>> RegisterAsync(RegisterViewModel model)
        {
            var payload = new
            {
                model.UserName,
                model.Email,
                model.Password,
                model.ConfirmPassword,
                RoleId = 2
            };

            var response = await _http.PostAsJsonAsync("/api/account/register", payload);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<UserViewModel>(_jsonOptions);
                return new ApiResult<UserViewModel> { Success = true, Data = data };
            }

            var error = await ReadErrorAsync(response);
            return new ApiResult<UserViewModel> { Success = false, Message = error.Message, Errors = error.Errors };
        }

        public async Task<ApiResult<LoginResponseViewModel>> LoginAsync(LoginViewModel model)
        {
            var response = await _http.PostAsJsonAsync("/api/account/login", model);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<LoginResponseViewModel>(_jsonOptions);
                return new ApiResult<LoginResponseViewModel> { Success = true, Data = data };
            }

            var error = await ReadErrorAsync(response);
            return new ApiResult<LoginResponseViewModel> { Success = false, Message = error.Message, Errors = error.Errors };
        }

        public async Task<ApiResult> UpdateUserAsync(string userName, UserUpdateViewModel model)
        {
            var response = await _http.PutAsJsonAsync($"/api/account/{userName}", model);
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message, Errors = error.Errors };
        }

        public async Task<ApiResult> DeleteUserAsync(string userName)
        {
            var response = await _http.DeleteAsync($"/api/account/{userName}");
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message, Errors = error.Errors };
        }

        public async Task<bool> CheckEmailAvailableAsync(string email, string? currentUserName = null)
        {
            var response = await _http.PostAsJsonAsync("/api/account/check-email",
                new { Email = email, CurrentUserName = currentUserName });
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>(_jsonOptions);
            return false;
        }

        public async Task<ApiResult<PasswordResetTokenResult>> RequestPasswordResetAsync(string email)
        {
            var response = await _http.PostAsJsonAsync("/api/account/password-reset-request", new { Email = email });
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<PasswordResetTokenResult>(_jsonOptions);
                return new ApiResult<PasswordResetTokenResult> { Success = true, Data = data };
            }

            var error = await ReadErrorAsync(response);
            return new ApiResult<PasswordResetTokenResult> { Success = false, Message = error.Message };
        }

        public async Task<ApiResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var payload = new
            {
                model.Token,
                model.UserName,
                model.NewPassword,
                model.ConfirmPassword
            };

            var response = await _http.PostAsJsonAsync("/api/account/password-reset", payload);
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true, Message = "Password reset successfully." };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message, Errors = error.Errors };
        }

        // ===== Categories =====

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var response = await _http.GetAsync("/api/categories");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>(_jsonOptions) ?? new();
            return new();
        }

        public async Task<CategoryViewModel?> GetCategoryAsync(string categoryCode)
        {
            var response = await _http.GetAsync($"/api/categories/{categoryCode}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CategoryViewModel>(_jsonOptions);
            return null;
        }

        public async Task<ApiResult<CategoryViewModel>> CreateCategoryAsync(CategoryCreateViewModel model)
        {
            var response = await _http.PostAsJsonAsync("/api/categories", model);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CategoryViewModel>(_jsonOptions);
                return new ApiResult<CategoryViewModel> { Success = true, Data = data };
            }

            var error = await ReadErrorAsync(response);
            return new ApiResult<CategoryViewModel> { Success = false, Message = error.Message };
        }

        public async Task<ApiResult> UpdateCategoryAsync(string categoryCode, CategoryCreateViewModel model)
        {
            var response = await _http.PutAsJsonAsync($"/api/categories/{categoryCode}", model);
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message };
        }

        public async Task<ApiResult> DeleteCategoryAsync(string categoryCode)
        {
            var response = await _http.DeleteAsync($"/api/categories/{categoryCode}");
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message };
        }

        // ===== Collection Items =====

        public async Task<List<CollectionItemViewModel>> GetItemsAsync()
        {
            var url = "/api/collectionitems";
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<CollectionItemViewModel>>(_jsonOptions) ?? new();
            return new();
        }

        public async Task<CollectionItemViewModel?> GetItemAsync(int id)
        {
            var response = await _http.GetAsync($"/api/collectionitems/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CollectionItemViewModel>(_jsonOptions);
            return null;
        }

        public async Task<ApiResult<CollectionItemViewModel>> CreateItemAsync(CollectionItemCreateViewModel model)
        {
            var response = await _http.PostAsJsonAsync("/api/collectionitems", model);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CollectionItemViewModel>(_jsonOptions);
                return new ApiResult<CollectionItemViewModel> { Success = true, Data = data };
            }

            var error = await ReadErrorAsync(response);
            return new ApiResult<CollectionItemViewModel> { Success = false, Message = error.Message, Errors = error.Errors };
        }

        public async Task<ApiResult> UpdateItemAsync(int id, CollectionItemUpdateViewModel model)
        {
            var response = await _http.PutAsJsonAsync($"/api/collectionitems/{id}", model);
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message, Errors = error.Errors };
        }

        public async Task<ApiResult> DeleteItemAsync(int id)
        {
            var response = await _http.DeleteAsync($"/api/collectionitems/{id}");
            if (response.IsSuccessStatusCode)
                return new ApiResult { Success = true };

            var error = await ReadErrorAsync(response);
            return new ApiResult { Success = false, Message = error.Message };
        }

        public async Task<List<CollectionItemViewModel>> SearchItemsAsync(string query)
        {
            var url = $"/api/collectionitems/search?q={Uri.EscapeDataString(query)}";
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<CollectionItemViewModel>>(_jsonOptions) ?? new();
            return new();
        }

        public async Task<List<CollectionItemViewModel>> GetRecentItemsAsync(int take = 10)
        {
            var response = await _http.GetAsync($"/api/collectionitems/recent?take={take}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<CollectionItemViewModel>>(_jsonOptions) ?? new();
            return new();
        }

        public async Task<BrowseResultViewModel?> GetBrowseItemsAsync(string search = "", string category = "", string sort = "newest", int page = 1, int pageSize = 24, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}",
                $"sort={Uri.EscapeDataString(sort)}"
            };
            
            if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrWhiteSpace(category)) queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice.Value}");
            if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice.Value}");

            var url = $"/api/collectionitems/browse?{string.Join("&", queryParams)}";
            var response = await _http.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BrowseResultViewModel>(_jsonOptions);
                
            return null;
        }

        public async Task<BrowseItemViewModel?> GetBrowseItemAsync(int id)
        {
            var response = await _http.GetAsync($"/api/collectionitems/browse/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BrowseItemViewModel>(_jsonOptions);
            return null;
        }

        // ===== Analytics =====

        public async Task<DashboardViewModel?> GetDashboardAsync(string? userName = null, string? role = null)
        {
            var url = $"/api/analytics/dashboard?userName={Uri.EscapeDataString(userName ?? string.Empty)}&role={Uri.EscapeDataString(role ?? string.Empty)}";
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<DashboardViewModel>(_jsonOptions);
            return null;
        }

        public async Task<List<LowStockItemViewModel>> GetLowStockAsync(string? userName = null, string? role = null)
        {
            var url = $"/api/analytics/lowstock?userName={Uri.EscapeDataString(userName ?? string.Empty)}&role={Uri.EscapeDataString(role ?? string.Empty)}";
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<LowStockItemViewModel>>(_jsonOptions) ?? new();
            return new();
        }

        public async Task<byte[]> ExportCsvAsync(string? userName = null, string? role = null)
        {
            var url = $"/api/analytics/export?userName={Uri.EscapeDataString(userName ?? string.Empty)}&role={Uri.EscapeDataString(role ?? string.Empty)}";
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsByteArrayAsync();
            return Array.Empty<byte>();
        }

        public async Task<CompletionViewModel?> GetCompletionAsync(string? userName = null, string? role = null)
        {
            var url = $"/api/analytics/completion?userName={Uri.EscapeDataString(userName ?? string.Empty)}&role={Uri.EscapeDataString(role ?? string.Empty)}";
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CompletionViewModel>(_jsonOptions);
            return null;
        }

        // ===== Helper =====

        private async Task<(string Message, List<string> Errors)> ReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorDoc = JsonSerializer.Deserialize<JsonElement>(content);

                var message = errorDoc.TryGetProperty("message", out var msgProp) ? msgProp.GetString() ?? "An error occurred." : "An error occurred.";
                var errors = new List<string>();

                if (errorDoc.TryGetProperty("errors", out var errProp) && errProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var err in errProp.EnumerateArray())
                    {
                        errors.Add(err.GetString() ?? "");
                    }
                }

                return (message, errors);
            }
            catch
            {
                return ("An error occurred while processing the request.", new List<string>());
            }
        }
    }
}
