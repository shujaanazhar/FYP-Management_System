using NexGen.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Iteration
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Details { get; set; }

    [Required]
    public int Number { get; set; }

    // Add a foreign key to FYP
    public string FYPName { get; set; }

    [ForeignKey("FYPName")]
    public virtual FYP FYP { get; set; }

    // Use a collection of ToDoItem instead of string
    public virtual ICollection<ToDoItem> ToDos { get; set; }
}
