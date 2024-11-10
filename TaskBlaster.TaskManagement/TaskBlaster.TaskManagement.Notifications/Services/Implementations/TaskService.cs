using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Notifications.Services.Interfaces;
using TaskBlaster.TaskManagement.DAL.Interfaces;

namespace TaskBlaster.TaskManagement.Notifications.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskWithNotificationDto>> GetTasksForNotifications()
    {
       return await _taskRepository.GetTasksForNotificationsAsync();
    }

    public async Task UpdateTaskNotifications()
    {
        await _taskRepository.UpdateTaskNotificationsAsync();
    }
}