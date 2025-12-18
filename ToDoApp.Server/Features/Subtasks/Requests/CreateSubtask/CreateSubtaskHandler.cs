using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Features.Subtasks.Requests.CreateSubtask;

public class CreateSubtaskHandler : IRequestHandler<CreateSubtaskRequest, CreateSubtaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public CreateSubtaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateSubtaskResponse> Handle(CreateSubtaskRequest request, CancellationToken cancellationToken)
    {
        // Verify task exists
        var taskExists = await _context.Tasks.AnyAsync(t => t.TaskId == request.TaskId, cancellationToken);
        if (!taskExists)
        {
            throw new InvalidOperationException($"Task with ID {request.TaskId} does not exist.");
        }

        var subtask = new Subtask
        {
            TaskId = request.TaskId,
            Description = request.Description,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.Subtasks.Add(subtask);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateSubtaskResponse
        {
            SubtaskId = subtask.SubtaskId,
            TaskId = subtask.TaskId,
            Description = subtask.Description,
            CompletedDate = subtask.CompletedDate,
            CreatedDate = subtask.CreatedDate
        };
    }
}

