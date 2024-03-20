using System.ComponentModel.DataAnnotations;

namespace NexGen.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(2)]
        public string Type { get; set; }
    }
}
