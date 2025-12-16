using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Server.Models;

public class Subtask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubtaskId { get; set; }

    [Required]
    public int TaskId { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime? CompletedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation property
    [ForeignKey(nameof(TaskId))]
    public virtual Task Task { get; set; } = null!;
}

