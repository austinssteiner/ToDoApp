using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Serve.DTOs;
using ToDoApp.Server.DTOs;
using ToDoApp.Server.Features.Subtasks.Requests.CreateSubtask;
using ToDoApp.Server.Features.Subtasks.Requests.DeleteSubtask;
using ToDoApp.Server.Features.Subtasks.Requests.UpdateSubtask;
using ToDoApp.Server.Features.Tasks.Requests.CreateTask;
using ToDoApp.Server.Features.Tasks.Requests.DeleteTask;
using ToDoApp.Server.Features.Tasks.Requests.UpdateTask;

namespace ToDoApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="dto">Task creation details</param>
    /// <returns>Created task information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateTaskResponse>> CreateTask([FromBody] CreateTaskDto dto)
    {
        try
        {
            var request = new CreateTaskRequest
            {
                UserId = dto.UserId,
                TaskName = dto.TaskName,
                Description = dto.Description,
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

    /// <summary>
    /// Updates an existing task
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="dto">Task update details</param>
    /// <returns>Updated task information</returns>
    [HttpPatch("{taskId}")]
    [ProducesResponseType(typeof(UpdateTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateTaskResponse>> SaveTask(int taskId, [FromBody] UpdateTaskDto dto)
    {
        try
        {
            var request = new UpdateTaskRequest
            {
                TaskId = taskId,
                TaskName = dto.TaskName,
                Description = dto.Description,
                CompletedDate = dto.CompletedDate
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a task and its subtasks
    /// </summary>
    /// <param name="dto">Task deletion details</param>
    /// <returns>Deletion result</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(DeleteTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteTaskResponse>> DeleteTask([FromBody] DeleteTaskDto dto)
    {
        try
        {
            var request = new DeleteTaskRequest
            {
                TaskId = dto.TaskId
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new subtask
    /// </summary>
    /// <param name="dto">Subtask creation details</param>
    /// <returns>Created subtask information</returns>
    [HttpPost("subtask")]
    [ProducesResponseType(typeof(CreateSubtaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSubtaskResponse>> CreateSubtask([FromBody] CreateSubtaskDto dto)
    {
        try
        {
            var request = new CreateSubtaskRequest
            {
                TaskId = dto.TaskId,
                Description = dto.Description,
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

    /// <summary>
    /// Updates an existing subtask
    /// </summary>
    /// <param name="subtaskId">Subtask ID</param>
    /// <param name="dto">Subtask update details</param>
    /// <returns>Updated subtask information</returns>
    [HttpPatch("subtask/{subtaskId}")]
    [ProducesResponseType(typeof(UpdateSubtaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateSubtaskResponse>> SaveSubtask(int subtaskId, [FromBody] UpdateSubtaskDto dto)
    {
        try
        {
            var request = new UpdateSubtaskRequest
            {
                SubtaskId = subtaskId,
                Description = dto.Description,
                CompletedDate = dto.CompletedDate
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a subtask
    /// </summary>
    /// <param name="dto">Subtask deletion details</param>
    /// <returns>Deletion result</returns>
    [HttpPost("subtask/delete")]
    [ProducesResponseType(typeof(DeleteSubtaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteSubtaskResponse>> DeleteSubtask([FromBody] DeleteSubtaskDto dto)
    {
        try
        {
            var request = new DeleteSubtaskRequest
            {
                SubtaskId = dto.SubtaskId
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

