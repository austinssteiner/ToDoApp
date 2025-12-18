using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;

namespace ToDoApp.Server.Features.Tasks.Requests.GetTask;

public class GetTaskHandler : IRequestHandler<GetTaskRequest, GetTaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public GetTaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetTaskResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.TaskId == request.TaskId, cancellationToken);

        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {request.TaskId} does not exist.");
        }

        return new GetTaskResponse
        {
            TaskId = task.TaskId,
            UserId = task.UserId,
            TaskName = task.TaskName,
            Description = task.Description,
            CompletedDate = task.CompletedDate,
            CreatedDate = task.CreatedDate,
            Subtasks = task.Subtasks.Select(s => new SubtaskDto
            {
                SubtaskId = s.SubtaskId,
                TaskId = s.TaskId,
                Description = s.Description,
                CompletedDate = s.CompletedDate,
                CreatedDate = s.CreatedDate
            }).ToList()
        };
    }
}

