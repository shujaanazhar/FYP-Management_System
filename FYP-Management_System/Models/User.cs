using System.ComponentModel.DataAnnotations;

namespace NexGen.Models
{
    public class User
    {
        [Key]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(2)]
        public string? Type { get; set; }
    }
}
