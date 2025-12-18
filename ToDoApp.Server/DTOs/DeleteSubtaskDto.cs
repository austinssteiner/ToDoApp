using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.DTOs;

public class DeleteSubtaskDto
{
    [Required(ErrorMessage = "SubtaskId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "SubtaskId must be greater than 0")]
    public int SubtaskId { get; set; }
}

