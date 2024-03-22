using FYP_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexGen.Models
{
    public class FYP
    {
        [Key]
        public string Name { get; set; }

        [Required]
        public int SupervisorId { get; set; }

        [Required]
        public string Details { get; set; }

        [Required]
        public string Domain { get; set; }

        // Navigation property to Supervisor
        public virtual Supervisor Supervisor { get; set; }
        public virtual ICollection<Student> Students { get; set; }

        public virtual ICollection<Iteration> iterations { get; set; }
    }
}
