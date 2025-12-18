using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Server.Data;
using ToDoApp.Server.DTOs;
using ToDoApp.Server.Features.Tasks.Requests.CreateTask;
using ToDoApp.Server.Features.Tasks.Requests.GetTasks;
using ToDoApp.Server.Features.Tasks.Requests.GetTask;
using ToDoApp.Server.Models;
using Xunit;

namespace ToDoApp.Server.Tests;

public class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TasksControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ToDoAppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ToDoAppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreatedTask()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var createTaskDto = new CreateTaskDto
        {
            UserId = userId,
            TaskName = "Test Task",
            Description = "Test Description",
            CreatedBy = userId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createTaskDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<CreateTaskResponse>();
        Assert.NotNull(task);
        Assert.Equal("Test Task", task.TaskName);
        Assert.Equal("Test Description", task.Description);
        Assert.Equal(userId, task.UserId);
        Assert.True(task.TaskId > 0);
        Assert.NotNull(task.CreatedDate);
    }

    [Fact]
    public async Task CreateTask_WithInvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            UserId = 99999, // Non-existent user
            TaskName = "Test Task",
            Description = "Test Description",
            CreatedBy = 99999
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createTaskDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_WithValidUserId_ReturnsTasksList()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var taskId = await CreateTestTaskAsync(userId);

        // Act
        var response = await _client.GetAsync($"/api/tasks/user/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<GetTasksResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.Tasks);
        Assert.Single(result.Tasks);
        Assert.Equal(taskId, result.Tasks[0].TaskId);
    }

    [Fact]
    public async Task GetTask_WithValidTaskId_ReturnsTask()
    {
        // Arrange
        var userId = await CreateTestUserAsync();
        var taskId = await CreateTestTaskAsync(userId);

        // Act
        var response = await _client.GetAsync($"/api/tasks/{taskId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<GetTaskResponse>();
        Assert.NotNull(task);
        Assert.Equal(taskId, task.TaskId);
        Assert.NotNull(task.Subtasks);
    }

    [Fact]
    public async Task GetTask_WithInvalidTaskId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateTestUserAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ToDoAppDbContext>();

        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            FirstName = "Test",
            LastName = "User",
            Role = RoleType.User,
            CreatedBy = 0,
            CreatedDate = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.UserId;
    }

    private async Task<int> CreateTestTaskAsync(int userId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ToDoAppDbContext>();

        var task = new Models.Task
        {
            UserId = userId,
            TaskName = "Test Task",
            Description = "Test Description",
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return task.TaskId;
    }
}

