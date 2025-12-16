using MediatR;

namespace ToDoApp.Server.Features.Subtasks.Requests.DeleteSubtask;

public class DeleteSubtaskRequest : IRequest<DeleteSubtaskResponse>
{
    public int SubtaskId { get; set; }
}

public class DeleteSubtaskResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

