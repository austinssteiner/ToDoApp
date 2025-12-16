using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Features.Users.Requests.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
{
    private readonly ToDoAppDbContext _context;

    public CreateUserHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);
        
        if (usernameExists)
        {
            throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
        }

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Role = request.Role,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            PasswordHash = passwordHash,
            CreatedBy = request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateUserResponse
        {
            UserId = user.UserId,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            CreatedDate = user.CreatedDate
        };
    }
}

