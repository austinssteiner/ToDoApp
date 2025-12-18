using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Server.DTOs;

public class UpdateSubtaskDto
{
    [MinLength(1, ErrorMessage = "Description cannot be empty")]
    public string? Description { get; set; }

    public DateTime? CompletedDate { get; set; }
}

