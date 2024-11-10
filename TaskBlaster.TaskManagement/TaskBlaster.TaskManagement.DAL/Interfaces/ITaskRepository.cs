using TaskBlaster.TaskManagement.Models;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;

namespace TaskBlaster.TaskManagement.DAL.Interfaces;

public interface ITaskRepository
{
    Task<Envelope<TaskDto>> GetPaginatedTasksByCriteriaAsync(TaskCriteriaQueryParams query);

    Task<TaskDetailsDto?> GetTaskByIdAsync(int taskId);

    Task<string?> GetTaskTitleByIdAsync(int taskId);

    Task<int> CreateNewTaskAsync(TaskInputModel task, string email);

    Task ArchiveTaskByIdAsync(int taskId);

    Task AssignUserToTaskAsync(int taskId, int userId);

    // assignedToId must be nullable
    Task UnassignUserFromTaskAsync(int taskId, int userId);

    Task UpdateTaskStatusAsync(int taskId, StatusInputModel inputModel);

    Task UpdateTaskPriorityAsync(int taskId, PriorityInputModel inputModel);

    Task<IEnumerable<TaskWithNotificationDto>> GetTasksForNotificationsAsync();
    
    Task UpdateTaskNotificationsAsync();
}