// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVaultAPI.Models
{
    public class CollectionItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        [Required]
        [StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        [Range(0, int.MaxValue)]
        public int StartingQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentQuantity { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageContentType { get; set; }

        public decimal? Price { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("UserName")]
        public ArcaneVaultUser? User { get; set; }

        public ICollection<CollectionItemCategory> CollectionItemCategories { get; set; } = new List<CollectionItemCategory>();
    }
}
