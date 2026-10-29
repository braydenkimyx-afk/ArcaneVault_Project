// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVaultAPI.Models
{
    public class ArcaneVaultUser
    {
        [Key]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("RoleId")]
        public ArcaneVaultUserRole? Role { get; set; }

        public ICollection<CollectionItem> CollectionItems { get; set; } = new List<CollectionItem>();
    }
}
