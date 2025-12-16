using MediatR;

namespace ToDoApp.Server.Features.Tasks.Requests.UpdateTask;

public class UpdateTaskRequest : IRequest<UpdateTaskResponse>
{
    public int TaskId { get; set; }
    public string? TaskName { get; set; }
    public string? Description { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class UpdateTaskResponse
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

