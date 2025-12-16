using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Server.DTOs;
using ToDoApp.Server.Features.Users.Requests.CreateUser;
using ToDoApp.Server.Features.Users.Requests.LoginUser;

namespace ToDoApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>User information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserDto dto)
    {
        try
        {
            var request = new LoginUserRequest
            {
                Username = dto.Username,
                Password = dto.Password
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="dto">User creation details</param>
    /// <returns>Created user information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var request = new CreateUserRequest
            {
                Role = dto.Role,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = dto.Password,
                CreatedBy = dto.CreatedBy
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

