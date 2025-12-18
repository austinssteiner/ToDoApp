using MediatR;

namespace ToDoApp.Server.Features.Tasks.Requests.GetTask;

public class GetTaskRequest : IRequest<GetTaskResponse>
{
    public int TaskId { get; set; }
}

public class GetTaskResponse
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<SubtaskDto> Subtasks { get; set; } = new();
}

public class SubtaskDto
{
    public int SubtaskId { get; set; }
    public int TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

