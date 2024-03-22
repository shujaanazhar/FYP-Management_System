using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexGen.Models
{
    public class Supervisor
    {
        [Key]
        [ForeignKey("User")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Domain { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<FYP> FYP { get; set; }
    }
}
