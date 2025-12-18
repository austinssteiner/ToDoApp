using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Features.Tasks.Requests.CreateTask;

public class CreateTaskHandler : IRequestHandler<CreateTaskRequest, CreateTaskResponse>
{
    private readonly ToDoAppDbContext _context;

    public CreateTaskHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTaskResponse> Handle(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var userExists = await _context.Users.AnyAsync(u => u.UserId == request.UserId, cancellationToken);
        if (!userExists)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} does not exist.");
        }

        var task = new Models.Task
        {
            UserId = request.UserId,
            TaskName = request.TaskName,
            Description = request.Description,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateTaskResponse
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

