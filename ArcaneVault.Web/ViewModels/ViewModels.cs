// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations;

namespace ArcaneVault.Web.ViewModels
{
    // ===== Account ViewModels =====

    public class RegisterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be 3-100 characters.")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(200)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class UserUpdateViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        public int RoleId { get; set; }
    }

    public class LoginResponseViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Registered Email")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ===== Category ViewModels =====

    public class CategoryViewModel
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category code is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Code must be 2-20 characters.")]
        [Display(Name = "Category Code")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;
    }

    // ===== Collection Item ViewModels =====

    public class CollectionItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<CategoryViewModel> Categories { get; set; } = new();
        public bool HasImage { get; set; }
        public decimal? Price { get; set; }
    }

    public class CollectionItemCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be 2-200 characters.")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Starting quantity must be non-negative.")]
        [Display(Name = "Starting Quantity")]
        public int StartingQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Current quantity must be non-negative.")]
        [Display(Name = "Current Quantity")]
        public int CurrentQuantity { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [Display(Name = "Price (optional)")]
        public decimal? Price { get; set; }

        [Display(Name = "Categories")]
        public List<string> CategoryCodes { get; set; } = new();

        public string? ImageBase64 { get; set; }
        public string? ImageContentType { get; set; }

        // Hidden — set server-side from claims
        public string UserName { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentQuantity > StartingQuantity)
            {
                yield return new ValidationResult(
                    "Current quantity cannot exceed starting quantity.",
                    new[] { nameof(CurrentQuantity) });
            }
        }
    }

    public class CollectionItemUpdateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Starting Quantity")]
        public int StartingQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Current Quantity")]
        public int CurrentQuantity { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [Display(Name = "Price (optional)")]
        public decimal? Price { get; set; }

        [Display(Name = "Categories")]
        public List<string> CategoryCodes { get; set; } = new();

        public string? ImageBase64 { get; set; }
        public string? ImageContentType { get; set; }
        public bool RemoveImage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentQuantity > StartingQuantity)
            {
                yield return new ValidationResult(
                    "Current quantity cannot exceed starting quantity.",
                    new[] { nameof(CurrentQuantity) });
            }
        }
    }

    public class BrowseItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<CategoryViewModel> Categories { get; set; } = new();
        public bool HasImage { get; set; }
        public decimal? Price { get; set; }
    }

    public class BrowseResultViewModel
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<BrowseItemViewModel> Items { get; set; } = new();
    }

    // ===== Analytics ViewModels =====

    public class DashboardViewModel
    {
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public int CategoriesUsed { get; set; }
        public double DepletionRate { get; set; }
        public List<CategoryDistributionViewModel> CategoryDistribution { get; set; } = new();
        public List<TopCategoryViewModel> TopCategories { get; set; } = new();
        public List<QuantityDepletionViewModel> QuantityDepletion { get; set; } = new();
        public List<LowStockItemViewModel> LowStockItems { get; set; } = new();
    }

    public class CategoryDistributionViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class TopCategoryViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class QuantityDepletionViewModel
    {
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
    }

    public class LowStockItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int StartingQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public double PercentRemaining { get; set; }
        public bool HasImage { get; set; }
    }

    // ===== Completion ViewModels =====

    public class CompletionViewModel
    {
        public int TotalCategories { get; set; }
        public List<CompletionCategoryViewModel> Categories { get; set; } = new();
        public double CompletionPercentage { get; set; }
    }

    public class CompletionCategoryViewModel
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool Owned { get; set; }
    }
}
