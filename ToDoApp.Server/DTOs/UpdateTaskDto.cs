using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.DTOs;

public class UpdateTaskDto
{
    [MaxLength(255, ErrorMessage = "TaskName cannot exceed 255 characters")]
    [MinLength(1, ErrorMessage = "TaskName cannot be empty")]
    public string? TaskName { get; set; }

    public string? Description { get; set; }

    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Indicates the client explicitly wants to update CompletedDate (including clearing it).
    /// </summary>
    public bool CompletedDateProvided { get; set; }
}
