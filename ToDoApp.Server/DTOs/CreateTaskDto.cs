namespace ToDoApp.Server.DTOs;

public class CreateTaskDto
{
    public int UserId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}

