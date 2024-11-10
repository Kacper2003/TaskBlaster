using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.Models;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;

namespace TaskBlaster.TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ICommentService _commentService;
    private readonly INotificationService _notificationService;

    public TasksController(ITaskService taskService, ICommentService commentService, INotificationService notificationService)
    {
        _taskService = taskService;
        _commentService = commentService;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Returns all tasks by a provided criteria as a paginated result
    /// </summary>
    /// <param name="query">A query which is used to paginate and filter the result</param>
    /// <returns>A filtered and paginated list of tasks</returns>
    [HttpGet("")]
    public async Task<ActionResult<Envelope<TaskDto>>> GetPaginatedTasksByCriteria([FromQuery] TaskCriteriaQueryParams query)
    {
        var tasks = await _taskService.GetPaginatedTasksByCriteriaAsync(query);

        return Ok(tasks);
    }

    /// <summary>
    /// Returns a single task by id
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <returns>A single task or null</returns>
    [HttpGet("{taskId}", Name = "GetTaskById")]
    public async Task<ActionResult<TaskDetailsDto?>> GetTaskById(int taskId)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);

        return Ok(task);
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="task">Input model used to populate the new task</param>
    [HttpPost("")]
    public async Task<ActionResult> CreateNewTask([FromBody] TaskInputModel task)
    {
        var email = User.FindFirst("email_address")?.Value
            ?? throw new InvalidOperationException("User email address not found in token");

        var taskId = await _taskService.CreateNewTaskAsync(task, email);

        return CreatedAtRoute("GetTaskById", new { taskId }, null);
    }
    
    /// <summary>
    /// Archives a task by id
    /// </summary>
    /// <param name="taskId">The id of the task which should be archived</param>
    [HttpDelete("{taskId}")]
    public async Task<ActionResult> ArchiveTaskById(int taskId)
    {
        await _taskService.ArchiveTaskByIdAsync(taskId);

        return NoContent();
    }

    /// <summary>
    /// Assigns a user from a task by id
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="userId">The id of the user which should be assigned</param>
    [HttpPatch("{taskId}/assign/{userId}")]
    public async Task<ActionResult> AssignUserToTask(int taskId, int userId)
    {
        await _taskService.AssignUserToTaskAsync(taskId, userId);

        await _notificationService.SendAssignedNotification(userId, taskId);

        return Ok();
    }
    
    /// <summary>
    /// Unassigns a user from a task by id
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="userId">The id of the user which should be unassigned</param>
    [HttpPatch("{taskId}/unassign/{userId}")]
    public async Task<ActionResult> UnassignUserFromTask(int taskId, int userId)
    {
        await _taskService.UnassignUserFromTaskAsync(taskId, userId);

        await _notificationService.SendUnassignedNotification(userId, taskId);

        return Ok();
    }

    /// <summary>
    /// Updates the status of a task, e.g. 'pending', 'completed'
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="inputModel">The input model associated with the status update</param>
    [HttpPatch("{taskId}/status")]
    public async Task<ActionResult> UpdateTaskStatus(int taskId, [FromBody] StatusInputModel inputModel)
    {
        await _taskService.UpdateTaskStatusAsync(taskId, inputModel);

        return Ok();
    }
    
    /// <summary>
    /// Updates the priority of a task, e.g. 'Critical', 'High'
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="inputModel">The input model associated with the priority update</param>
    [HttpPatch("{taskId}/priority")]
    public async Task<ActionResult> UpdateTaskPriority(int taskId, [FromBody] PriorityInputModel inputModel)
    {
        await _taskService.UpdateTaskPriorityAsync(taskId, inputModel);

        return Ok();
    }

    /// <summary>
    /// Gets all comments associated with a task
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <returns>A list of comments</returns>
    [HttpGet("{taskId}/comments")]
    public async Task<ActionResult> GetCommentsAssociatedWithTask(int taskId)
    {
        var comments = await _commentService.GetCommentsAssociatedWithTaskAsync(taskId);

        if (comments == null || !comments.Any())
        {
            return Ok(new List<CommentDto>());
        }

        return Ok(comments);
    }

    /// <summary>
    /// Adds a single comment to a task
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="inputModel">The input model for the comment</param>
    [HttpPost("{taskId}/comments")]
    public async Task<ActionResult> AddCommentToTask(int taskId, [FromBody] CommentInputModel inputModel)
    {
        var email = User.FindFirst("email_address")?.Value
            ?? throw new InvalidOperationException("User email address not found in token");
            
        await _commentService.AddCommentToTaskAsync(taskId, inputModel, email);

        return Ok();
    }
    
    /// <summary>
    /// Removes a comment from a task
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="commentId">The id of the comment</param>
    [HttpDelete("{taskId}/comments/{commentId}")]
    public async Task<ActionResult> RemoveCommentFromTask(int taskId, int commentId)
    {
        await _commentService.RemoveCommentFromTaskAsync(taskId, commentId);

        return Ok();
    }
}
