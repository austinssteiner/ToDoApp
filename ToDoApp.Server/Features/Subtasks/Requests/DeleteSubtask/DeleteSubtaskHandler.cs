using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;

namespace ToDoApp.Server.Features.Subtasks.Requests.DeleteSubtask;

public class DeleteSubtaskHandler : IRequestHandler<DeleteSubtaskRequest, DeleteSubtaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public DeleteSubtaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteSubtaskResponse> Handle(DeleteSubtaskRequest request, CancellationToken cancellationToken)
    {
        var subtask = await _context.Subtasks.FindAsync(new object[] { request.SubtaskId }, cancellationToken);

        if (subtask == null)
        {
            throw new InvalidOperationException($"Subtask with ID {request.SubtaskId} does not exist.");
        }

        _context.Subtasks.Remove(subtask);
        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteSubtaskResponse
        {
            Success = true,
            Message = $"Subtask {request.SubtaskId} has been deleted successfully."
        };
    }
}

