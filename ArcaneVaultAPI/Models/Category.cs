// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations;

namespace ArcaneVaultAPI.Models
{
    public class Category
    {
        [Key]
        [StringLength(20)]
        public string CategoryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Navigation property
        public ICollection<CollectionItemCategory> CollectionItemCategories { get; set; } = new List<CollectionItemCategory>();
    }
}
