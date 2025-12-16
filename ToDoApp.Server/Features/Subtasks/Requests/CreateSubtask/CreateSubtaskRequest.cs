using MediatR;

namespace ToDoApp.Server.Features.Subtasks.Requests.CreateSubtask;

public class CreateSubtaskRequest : IRequest<CreateSubtaskResponse>
{
    public int TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}

public class CreateSubtaskResponse
{
    public int SubtaskId { get; set; }
    public int TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

