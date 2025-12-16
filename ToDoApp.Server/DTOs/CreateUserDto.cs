using ToDoApp.Server.Models;

namespace ToDoApp.Server.DTOs;

public class CreateUserDto
{
    public RoleType Role { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
}

