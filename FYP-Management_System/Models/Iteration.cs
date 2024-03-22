using NexGen.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FYP_Management_System.Models
{
    public class Iteration
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Details { get; set; }

        [Required]
        public int Number { get; set; }

        public virtual ICollection<String> To_do { get; set; }
    }
}