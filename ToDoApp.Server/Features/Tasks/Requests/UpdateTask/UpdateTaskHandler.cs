using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;

namespace ToDoApp.Server.Features.Tasks.Requests.UpdateTask;

public class UpdateTaskHandler : IRequestHandler<UpdateTaskRequest, UpdateTaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public UpdateTaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateTaskResponse> Handle(UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FindAsync(new object[] { request.TaskId }, cancellationToken);
        
        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {request.TaskId} does not exist.");
        }

        // Update only provided fields
        if (request.TaskName != null)
        {
            task.TaskName = request.TaskName;
        }

        if (request.Description != null)
        {
            task.Description = request.Description;
        }

        if (request.CompletedDate.HasValue)
        {
            task.CompletedDate = request.CompletedDate.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateTaskResponse
        {
            TaskId = task.TaskId,
            UserId = task.UserId,
            TaskName = task.TaskName,
            Description = task.Description,
            CompletedDate = task.CompletedDate,
            CreatedDate = task.CreatedDate
        };
    }
}

