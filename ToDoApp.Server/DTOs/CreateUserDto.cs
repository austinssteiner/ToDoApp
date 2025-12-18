using System.ComponentModel.DataAnnotations;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "Role is required")]
    public RoleType Role { get; set; }

    [Required(ErrorMessage = "FirstName is required")]
    [MaxLength(255, ErrorMessage = "FirstName cannot exceed 255 characters")]
    [MinLength(1, ErrorMessage = "FirstName cannot be empty")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required")]
    [MaxLength(255, ErrorMessage = "LastName cannot exceed 255 characters")]
    [MinLength(1, ErrorMessage = "LastName cannot be empty")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedBy is required")]
    [Range(0, int.MaxValue, ErrorMessage = "CreatedBy must be 0 or greater")]
    public int CreatedBy { get; set; }
}

