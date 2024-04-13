using NexGen.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NexGen.Models
{
    public class Iteration
    {
        [Key]
        public string Name { get; set; }
        [Required]
        public string Details { get; set; }
        public string Status { get; set; } = "Ongoing";
        public DateOnly DueDate { get; set; }
        public string FYPName { get; set; }
        public string Task1 {  get; set; }
        public string Task2 { get; set; }
        public string Task3 { get; set; }

        [ForeignKey("FYPName")]
        public virtual FYP FYP { get; set; }
        [NotMapped]
        public List<string>? Tasks { get; set; }
    }

}
