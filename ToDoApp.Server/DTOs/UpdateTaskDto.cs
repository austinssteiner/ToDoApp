namespace ToDoApp.Server.DTOs;

public class UpdateTaskDto
{
    public string? TaskName { get; set; }
    public string? Description { get; set; }
    public DateTime? CompletedDate { get; set; }
}

