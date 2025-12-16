namespace ToDoApp.Server.DTOs;

public class CreateSubtaskDto
{
    public int TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}

