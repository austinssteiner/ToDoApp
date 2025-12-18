using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.DTOs;

public class CreateSubtaskDto
{
    [Required(ErrorMessage = "TaskId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "TaskId must be greater than 0")]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [MinLength(1, ErrorMessage = "Description cannot be empty")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedBy is required")]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedBy must be greater than 0")]
    public int CreatedBy { get; set; }
}

