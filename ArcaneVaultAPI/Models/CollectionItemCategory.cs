// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVaultAPI.Models
{
    public class CollectionItemCategory
    {
        public int ItemId { get; set; }

        public string CategoryCode { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("ItemId")]
        public CollectionItem? CollectionItem { get; set; }

        [ForeignKey("CategoryCode")]
        public Category? Category { get; set; }
    }
}
