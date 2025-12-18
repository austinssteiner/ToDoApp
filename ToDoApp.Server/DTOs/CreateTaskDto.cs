using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "UserId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "TaskName is required")]
    [MaxLength(255, ErrorMessage = "TaskName cannot exceed 255 characters")]
    [MinLength(1, ErrorMessage = "TaskName cannot be empty")]
    public string TaskName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedBy is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy must be greater than 0")]
    public int CreatedBy { get; set; }
}

