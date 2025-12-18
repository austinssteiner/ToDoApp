using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.DTOs;

public class DeleteTaskDto
{
    [Required(ErrorMessage = "TaskId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "TaskId must be greater than 0")]
    public int TaskId { get; set; }
}

