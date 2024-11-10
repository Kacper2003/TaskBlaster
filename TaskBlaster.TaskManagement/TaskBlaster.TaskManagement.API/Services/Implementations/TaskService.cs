using System.Xml.Schema;
using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.Models;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;
using TaskBlaster.TaskManagement.Models.Exceptions;

namespace TaskBlaster.TaskManagement.API.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    
    public async Task<Envelope<TaskDto>> GetPaginatedTasksByCriteriaAsync(TaskCriteriaQueryParams query)
    {
        return await _taskRepository.GetPaginatedTasksByCriteriaAsync(query);
    }

    public async Task<TaskDetailsDto?> GetTaskByIdAsync(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        return await _taskRepository.GetTaskByIdAsync(taskId) ?? throw new ResourceNotFoundException($"Task with ID {taskId} not found");
    }

    public async Task<int> CreateNewTaskAsync(TaskInputModel task, string email)
    {
        return await _taskRepository.CreateNewTaskAsync(task, email);
    }

    public async Task ArchiveTaskByIdAsync(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        await _taskRepository.ArchiveTaskByIdAsync(taskId);
    }

    public async Task AssignUserToTaskAsync(int taskId, int userId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than 0");
        }
        await _taskRepository.AssignUserToTaskAsync(taskId, userId);
    }

    public async Task UnassignUserFromTaskAsync(int taskId, int userId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than 0");
        }
        await _taskRepository.UnassignUserFromTaskAsync(taskId, userId);
    }

    public async Task UpdateTaskStatusAsync(int taskId, StatusInputModel inputModel)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        await _taskRepository.UpdateTaskStatusAsync(taskId, inputModel);
    }

    public async Task UpdateTaskPriorityAsync(int taskId, PriorityInputModel inputModel)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Task ID must be greater than 0");
        }
        await _taskRepository.UpdateTaskPriorityAsync(taskId, inputModel);
    }
}