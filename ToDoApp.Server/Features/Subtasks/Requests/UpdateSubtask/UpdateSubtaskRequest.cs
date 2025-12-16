using MediatR;

namespace ToDoApp.Server.Features.Subtasks.Requests.UpdateSubtask;

public class UpdateSubtaskRequest : IRequest<UpdateSubtaskResponse>
{
    public int SubtaskId { get; set; }
    public string? Description { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class UpdateSubtaskResponse
{
    public int SubtaskId { get; set; }
    public int TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

