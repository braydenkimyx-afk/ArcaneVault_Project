// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

namespace ArcaneVaultAPI.DTOs
{
    // ===== Account DTOs =====

    public class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public int RoleId { get; set; } = 2; // Default to User role
    }

    public class LoginDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class UserUpdateDto
    {
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class PasswordResetRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class PasswordResetDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CheckEmailDto
    {
        public string Email { get; set; } = string.Empty;
        public string? CurrentUserName { get; set; }
    }

    // ===== Category DTOs =====

    public class CategoryDto
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CategoryCreateDto
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    // ===== Collection Item DTOs =====

    public class CollectionItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new();
        public bool HasImage { get; set; }
        public decimal? Price { get; set; }
    }

    public class CollectionItemCreateDto
    {
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> CategoryCodes { get; set; } = new();
        public string? ImageBase64 { get; set; }
        public string? ImageContentType { get; set; }
        public decimal? Price { get; set; }
    }

    public class CollectionItemUpdateDto
    {
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public List<string> CategoryCodes { get; set; } = new();
        public string? ImageBase64 { get; set; }
        public string? ImageContentType { get; set; }
        public bool RemoveImage { get; set; }
        public decimal? Price { get; set; }
    }

    public class BrowseItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new();
        public bool HasImage { get; set; }
        public decimal? Price { get; set; }
    }

    public class BrowseResponseDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<BrowseItemDto> Items { get; set; } = new();
    }

    // ===== Analytics DTOs =====

    public class DashboardDto
    {
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public int CategoriesUsed { get; set; }
        public double DepletionRate { get; set; }
        public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();
        public List<TopCategoryDto> TopCategories { get; set; } = new();
        public List<QuantityDepletionDto> QuantityDepletion { get; set; } = new();
        public List<LowStockItemDto> LowStockItems { get; set; } = new();
    }

    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class TopCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class QuantityDepletionDto
    {
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
    }

    public class LowStockItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public double PercentRemaining { get; set; }
        public bool HasImage { get; set; }
    }

    // ===== Completion DTOs =====

    public class CompletionDto
    {
        public int TotalCategories { get; set; }
        public List<CompletionCategoryDto> Categories { get; set; } = new();
        public double CompletionPercentage { get; set; }
    }

    public class CompletionCategoryDto
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool Owned { get; set; }
    }

    // ===== Error DTOs =====

    public class ErrorResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
