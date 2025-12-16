using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;

namespace ToDoApp.Server.Features.Tasks.Requests.DeleteTask;

public class DeleteTaskHandler : IRequestHandler<DeleteTaskRequest, DeleteTaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public DeleteTaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteTaskResponse> Handle(DeleteTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.TaskId == request.TaskId, cancellationToken);

        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {request.TaskId} does not exist.");
        }

        // Subtasks will be deleted automatically due to cascade delete
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteTaskResponse
        {
            Success = true,
            Message = $"Task {request.TaskId} and its subtasks have been deleted successfully."
        };
    }
}

