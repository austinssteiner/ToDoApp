using MediatR;

namespace ToDoApp.Server.Features.Tasks.Requests.CreateTask;

public class CreateTaskRequest : IRequest<CreateTaskResponse>
{
    public int UserId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}

public class CreateTaskResponse
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

