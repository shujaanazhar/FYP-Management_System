using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ToDoItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Task { get; set; }

    public int IterationId { get; set; }

    [ForeignKey("IterationId")]
    public virtual Iteration Iteration { get; set; }
}
