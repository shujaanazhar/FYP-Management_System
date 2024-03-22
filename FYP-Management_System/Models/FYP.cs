using System.ComponentModel.DataAnnotations;

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

        public virtual ICollection<Iteration> Iterations { get; set; }
    }
}
