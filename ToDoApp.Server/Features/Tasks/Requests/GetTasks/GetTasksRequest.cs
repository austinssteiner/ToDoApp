using MediatR;

namespace ToDoApp.Server.Features.Tasks.Requests.GetTasks;

public class GetTasksRequest : IRequest<GetTasksResponse>
{
    public int UserId { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public string? SearchTerm { get; set; }
    public bool? Completed { get; set; }
}

public class GetTasksResponse
{
    public List<TaskDto> Tasks { get; set; } = new();
    public int TotalCount { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public bool HasMore { get; set; }
}

public class TaskDto
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
