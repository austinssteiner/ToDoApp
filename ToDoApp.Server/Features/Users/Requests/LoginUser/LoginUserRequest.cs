using MediatR;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Features.Users.Requests.LoginUser;

public class LoginUserRequest : IRequest<LoginUserResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginUserResponse
{
    public int UserId { get; set; }
    public RoleType Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

