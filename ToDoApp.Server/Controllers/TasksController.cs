using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Server.DTOs;
using ToDoApp.Server.Features.Subtasks.Requests.CreateSubtask;
using ToDoApp.Server.Features.Subtasks.Requests.DeleteSubtask;
using ToDoApp.Server.Features.Subtasks.Requests.UpdateSubtask;
using ToDoApp.Server.Features.Tasks.Requests.CreateTask;
using ToDoApp.Server.Features.Tasks.Requests.DeleteTask;
using ToDoApp.Server.Features.Tasks.Requests.GetTask;
using ToDoApp.Server.Features.Tasks.Requests.GetTasks;
using ToDoApp.Server.Features.Tasks.Requests.UpdateTask;

namespace ToDoApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Task endpoints (supports pagination, filtering, sorting) powered by MediatR handlers.
/// </summary>
public class TasksController : ControllerBase
{
    private const int MaxPageSize = 100;
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all tasks for a user with optional pagination, sorting, and filtering.
    /// </summary>
    /// <param name="userId">User ID (required)</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size (defaults to all, max 100)</param>
    /// <param name="sortBy">Sort by `createdDate` (default), `name`, or `completedDate`</param>
    /// <param name="sortDirection">Sort direction `asc` or `desc` (default)</param>
    /// <param name="searchTerm">Case-insensitive search against task name/description</param>
    /// <param name="completed">Filter by completion status (true = only completed, false = only active)</param>
    /// <remarks>
    /// Adds backend support for production concerns without breaking existing clients:
    /// - Defaults to returning the full list if pagination parameters are omitted.
    /// - Returns metadata (totalCount, hasMore) so the frontend can enable paging later.
    /// - Validates inputs to return ProblemDetails instead of unhandled errors.
    /// </remarks>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(GetTasksResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetTasksResponse>> GetTasks(
        int userId,
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? completed = null)
    {
        if (userId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "UserId must be greater than 0",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (pageNumber.HasValue && pageNumber <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "PageNumber must be greater than 0 when provided",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (pageSize.HasValue && (pageSize <= 0 || pageSize > MaxPageSize))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = $"PageSize must be between 1 and {MaxPageSize} when provided",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var allowedSortFields = new[] { "createddate", "name", "completeddate" };
            if (!allowedSortFields.Contains(sortBy.ToLower()))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Validation error",
                    Detail = "SortBy must be one of: createdDate, name, completedDate",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        var request = new GetTasksRequest
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection,
            SearchTerm = searchTerm,
            Completed = completed
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Gets a single task by ID with its subtasks.
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <returns>Task information with subtasks</returns>
    /// <response code="404">Task not found for the provided ID</response>
    [HttpGet("{taskId}")]
    [ProducesResponseType(typeof(GetTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTaskResponse>> GetTask(int taskId)
    {
        if (taskId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "TaskId must be greater than 0",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var request = new GetTaskRequest
        {
            TaskId = taskId
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="dto">Task creation details</param>
    /// <returns>Created task information</returns>
    /// <response code="400">Validation errors with field-level details</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateTaskResponse>> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

    /// <summary>
    /// Updates an existing task.
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
        if (taskId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "TaskId must be greater than 0",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new UpdateTaskRequest
        {
            TaskId = taskId,
            TaskName = dto.TaskName,
            Description = dto.Description,
            CompletedDate = dto.CompletedDate,
            CompletedDateProvided = dto.CompletedDateProvided
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a task and its subtasks.
    /// </summary>
    /// <param name="dto">Task deletion details</param>
    /// <returns>Deletion result</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(DeleteTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteTaskResponse>> DeleteTask([FromBody] DeleteTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new DeleteTaskRequest
        {
            TaskId = dto.TaskId
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new subtask.
    /// </summary>
    /// <param name="dto">Subtask creation details</param>
    /// <returns>Created subtask information</returns>
    [HttpPost("subtask")]
    [ProducesResponseType(typeof(CreateSubtaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSubtaskResponse>> CreateSubtask([FromBody] CreateSubtaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new CreateSubtaskRequest
        {
            TaskId = dto.TaskId,
            Description = dto.Description,
            CreatedBy = dto.CreatedBy
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing subtask.
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
        if (subtaskId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "SubtaskId must be greater than 0",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new UpdateSubtaskRequest
        {
            SubtaskId = subtaskId,
            Description = dto.Description,
            CompletedDate = dto.CompletedDate
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a subtask.
    /// </summary>
    /// <param name="dto">Subtask deletion details</param>
    /// <returns>Deletion result</returns>
    [HttpPost("subtask/delete")]
    [ProducesResponseType(typeof(DeleteSubtaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteSubtaskResponse>> DeleteSubtask([FromBody] DeleteSubtaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var request = new DeleteSubtaskRequest
        {
            SubtaskId = dto.SubtaskId
        };

        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
