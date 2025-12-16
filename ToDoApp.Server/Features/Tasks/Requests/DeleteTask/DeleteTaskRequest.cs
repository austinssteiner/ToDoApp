using MediatR;

namespace ToDoApp.Server.Features.Tasks.Requests.DeleteTask;

public class DeleteTaskRequest : IRequest<DeleteTaskResponse>
{
    public int TaskId { get; set; }
}

public class DeleteTaskResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

