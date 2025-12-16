using MediatR;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Features.Users.Requests.CreateUser;

public class CreateUserRequest : IRequest<CreateUserResponse>
{
    public RoleType Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}

public class CreateUserResponse
{
    public int UserId { get; set; }
    public RoleType Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

