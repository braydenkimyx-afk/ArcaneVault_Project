// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations;

namespace ArcaneVaultAPI.Models
{
    public class ArcaneVaultUserRole
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        // Navigation property
        public ICollection<ArcaneVaultUser> Users { get; set; } = new List<ArcaneVaultUser>();
    }
}
