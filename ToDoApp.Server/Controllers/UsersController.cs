using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Server.DTOs;
using ToDoApp.Server.Features.Users.Requests.CreateUser;
using ToDoApp.Server.Features.Users.Requests.LoginUser;

namespace ToDoApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
/// <summary>
/// User account endpoints (currently simple login/create; token issuing can be layered later).
/// </summary>
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Logs in a user with username and password.
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>User information</returns>
    /// <remarks>
    /// Placeholder for a future JWT/session implementation. On successful login, callers
    /// receive the user record; production-ready auth would issue tokens here.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new LoginUserRequest
        {
            Username = dto.Username,
            Password = dto.Password
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto">User creation details</param>
    /// <returns>Created user information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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
}
