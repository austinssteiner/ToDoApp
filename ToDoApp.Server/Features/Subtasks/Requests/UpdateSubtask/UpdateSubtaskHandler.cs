using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;

namespace ToDoApp.Server.Features.Subtasks.Requests.UpdateSubtask;

public class UpdateSubtaskHandler : IRequestHandler<UpdateSubtaskRequest, UpdateSubtaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public UpdateSubtaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateSubtaskResponse> Handle(UpdateSubtaskRequest request, CancellationToken cancellationToken)
    {
        var subtask = await _context.Subtasks.FindAsync(new object[] { request.SubtaskId }, cancellationToken);

        if (subtask == null)
        {
            throw new InvalidOperationException($"Subtask with ID {request.SubtaskId} does not exist.");
        }

        // Update only provided fields
        if (request.Description != null)
        {
            subtask.Description = request.Description;
        }

        // Allow toggling completion both ways (set timestamp or clear it)
        subtask.CompletedDate = request.CompletedDate;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateSubtaskResponse
        {
            SubtaskId = subtask.SubtaskId,
            TaskId = subtask.TaskId,
            Description = subtask.Description,
            CompletedDate = subtask.CompletedDate,
            CreatedDate = subtask.CreatedDate
        };
    }
}

