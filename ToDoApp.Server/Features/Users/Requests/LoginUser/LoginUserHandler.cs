using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Features.Users.Requests.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
{
    private readonly ToDoAppDbContext _context;

    public LoginUserHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        // Verify password
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        
        if (!isPasswordValid)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        return new LoginUserResponse
        {
            UserId = user.UserId,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username
        };
    }
}

