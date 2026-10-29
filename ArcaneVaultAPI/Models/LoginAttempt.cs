// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArcaneVaultAPI.Models
{
    public class LoginAttempt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        public bool Successful { get; set; }

        // Navigation property
        [ForeignKey("UserName")]
        public ArcaneVaultUser? User { get; set; }
    }
}
