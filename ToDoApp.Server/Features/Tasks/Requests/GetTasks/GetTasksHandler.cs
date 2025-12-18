using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Data;

namespace ToDoApp.Server.Features.Tasks.Requests.GetTasks;

public class GetTasksHandler : IRequestHandler<GetTasksRequest, GetTasksResponse>
{
    private readonly ToDoAppDbContext _context;

    public GetTasksHandler(ToDoAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetTasksResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var baseQuery = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == request.UserId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            baseQuery = baseQuery.Where(t =>
                t.TaskName.ToLower().Contains(search) ||
                t.Description.ToLower().Contains(search));
        }

        if (request.Completed.HasValue)
        {
            baseQuery = request.Completed.Value
                ? baseQuery.Where(t => t.CompletedDate.HasValue)
                : baseQuery.Where(t => !t.CompletedDate.HasValue);
        }

        var sortBy = request.SortBy?.ToLower() ?? "createddate";
        var sortDirection = request.SortDirection?.ToLower() == "asc" ? "asc" : "desc";

        var orderedQuery = sortBy switch
        {
            "name" => sortDirection == "asc"
                ? baseQuery.OrderBy(t => t.TaskName)
                : baseQuery.OrderByDescending(t => t.TaskName),
            "completeddate" => sortDirection == "asc"
                ? baseQuery.OrderBy(t => t.CompletedDate.HasValue).ThenBy(t => t.CompletedDate)
                : baseQuery.OrderByDescending(t => t.CompletedDate.HasValue).ThenByDescending(t => t.CompletedDate),
            _ => sortDirection == "asc"
                ? baseQuery.OrderBy(t => t.CreatedDate)
                : baseQuery.OrderByDescending(t => t.CreatedDate)
        };

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        IQueryable<Models.Task> pagedQuery = orderedQuery;

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            var skip = (request.PageNumber.Value - 1) * request.PageSize.Value;
            pagedQuery = orderedQuery
                .Skip(skip)
                .Take(request.PageSize.Value);
        }

        var tasks = await pagedQuery
            .Select(t => new TaskDto
            {
                TaskId = t.TaskId,
                UserId = t.UserId,
                TaskName = t.TaskName,
                Description = t.Description,
                CompletedDate = t.CompletedDate,
                CreatedDate = t.CreatedDate,
                Subtasks = t.Subtasks.Select(s => new SubtaskDto
                {
                    SubtaskId = s.SubtaskId,
                    TaskId = s.TaskId,
                    Description = s.Description,
                    CompletedDate = s.CompletedDate,
                    CreatedDate = s.CreatedDate
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new GetTasksResponse
        {
            Tasks = tasks,
            TotalCount = totalCount,
            PageNumber = request.PageNumber ?? 1,
            PageSize = request.PageSize ?? totalCount,
            HasMore = request.PageNumber.HasValue && request.PageSize.HasValue
                ? (request.PageNumber.Value * request.PageSize.Value) < totalCount
                : false
        };
    }
}
