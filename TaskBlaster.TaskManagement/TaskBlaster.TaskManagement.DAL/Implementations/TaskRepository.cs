using Microsoft.EntityFrameworkCore;
using TaskBlaster.TaskManagement.DAL.Interfaces;
using TaskBlaster.TaskManagement.Models;
using TaskBlaster.TaskManagement.Models.Dtos;
using TaskBlaster.TaskManagement.Models.InputModels;
using TaskBlaster.TaskManagement.Models.Exceptions;

namespace TaskBlaster.TaskManagement.DAL.Implementations;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public TaskRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ArchiveTaskByIdAsync(int taskId)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId)
                    ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        task.IsArchived = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task AssignUserToTaskAsync(int taskId, int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId)
                    ?? throw new ResourceNotFoundException($"User with id {userId} not found");

        var task = await _dbContext.Tasks.FindAsync(taskId)
                    ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        if (task.IsArchived)
        {
            throw new InvalidOperationException($"Task with id {taskId} is archived and cannot have users assigned.");
        }
        if (task.AssignedToId == userId)
        {
            throw new InvalidOperationException($"User with id {userId} is already assigned to task with id {taskId}.");
        }

        task.AssignedToId = userId;

        await _dbContext.SaveChangesAsync(); // Ensure save completes before returning
    }


    public async Task<int> CreateNewTaskAsync(TaskInputModel taskInputModel, string email)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == email)
            ?? throw new ResourceNotFoundException($"User with email {email} not found");

        // Check if request wants to assign the task to a user
        var assignedToUser = taskInputModel.AssignedToUser != null
            // If so, find the user by email
            ? await _dbContext.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == taskInputModel.AssignedToUser)
                ?? throw new ResourceNotFoundException($"User with email {taskInputModel.AssignedToUser} not found")
            // If not, assign null
            : null;

        // Initialize the TaskNotification
        var taskNotification = new Entities.TaskNotification
        {
            DueDateNotificationSent = false,
            DayAfterNotificationSent = false,
            LastNotificationDate = null
        };

        var newTask = new Entities.Task
        {
            Title = taskInputModel.Title,
            Description = taskInputModel.Description,
            CreatedAt = DateTime.UtcNow,
            DueDate = taskInputModel.DueDate,
            PriorityId = taskInputModel.PriorityId,
            StatusId = taskInputModel.StatusId,
            AssignedToId = assignedToUser?.Id,
            CreatedById = user.Id,
            TaskNotifications = new List<Entities.TaskNotification> { taskNotification } 
        };
        
        _dbContext.Tasks.Add(newTask);
        await _dbContext.SaveChangesAsync(); 

        return newTask.Id; // Directly return the ID after save completes
    }


    public async Task<Envelope<TaskDto>> GetPaginatedTasksByCriteriaAsync(TaskCriteriaQueryParams query)
    {
        // Validate PageNumber and PageSize, defaulting to 1 and 10 
        int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
        int pageSize = query.PageSize > 0 ? query.PageSize : 10;

        // Build the query, applying the IsArchived filter and SearchValue filter if present
        var taskQuery = _dbContext.Tasks
            .Where(t => !t.IsArchived) // Exclude archived tasks
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.SearchValue))
        {
            // Make the search case-insensitive by converting both sides to lowercase
            var searchValue = query.SearchValue.ToLower();
            taskQuery = taskQuery.Where(t => EF.Functions.Like(t.Title.ToLower(), $"%{searchValue}%"));
        }

        // Get the total count of items matching the filter
        var totalCount = await taskQuery.CountAsync();

        // Apply pagination
        var tasks = await taskQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status.Name,
                DueDate = t.DueDate,
                AssignedToUser = t.AssignedTo != null ? t.AssignedTo.FullName : null
            })
            .ToListAsync();

        // Build the paginated response
        var envelope = new Envelope<TaskDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            MaxCount = totalCount,
            Items = tasks
        };

        return envelope;
    }


    public async Task<TaskDetailsDto?> GetTaskByIdAsync(int taskId)
    {
        var task = await _dbContext.Tasks
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Include(t => t.Comments)
            .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == taskId); // Fetch the task without filtering for IsArchived

        // Check if the task was found and if it is archived
        if (task == null)
        {
            throw new ResourceNotFoundException($"Task with ID {taskId} not found.");
        }

        if (task.IsArchived)
        {
            throw new InvalidOperationException($"Task with ID {taskId} is archived and cannot be accessed.");
        }

        // If not archived, project to TaskDetailsDto
        return new TaskDetailsDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.Name,
            Priority = task.Priority.Name,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CreatedBy = task.CreatedBy.FullName,
            AssignedToUser = task.AssignedTo?.FullName,
            Tags = task.TaskTags.Select(tt => tt.Tag.Name).ToList(),
            Comments = task.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Author = c.Author,
                ContentAsMarkdown = c.ContentAsMarkdown,
                CreatedDate = c.CreatedDate
            }).ToList()
        };
    }



    public async Task<string?> GetTaskTitleByIdAsync(int taskId)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId);

        return task?.Title;
    }

    public async Task<IEnumerable<TaskWithNotificationDto>> GetTasksForNotificationsAsync()
    {
        // Get the current UTC date (without time component)
        var currentUtcDate = DateTime.UtcNow.Date;

        // Calculate yesterday's date
        var yesterdayUtcDate = currentUtcDate.AddDays(-1);

        // Query tasks that:
        // - Have a due date
        // - Are assigned to a user
        // - Are not archived
        // - Either due today and DueDateNotificationSent is false
        // - Or due yesterday and DayAfterNotificationSent is false
        var tasksToNotify = await _dbContext.Tasks
            .Include(t => t.TaskNotifications)
            .Include(t => t.AssignedTo)
            .Where(t => 
                t.DueDate.HasValue &&
                t.AssignedToId != null &&
                !t.IsArchived &&
                (
                    (t.DueDate.Value.Date == currentUtcDate && !t.TaskNotifications.Any(tn => tn.DueDateNotificationSent)) ||
                    (t.DueDate.Value.Date == yesterdayUtcDate && !t.TaskNotifications.Any(tn => tn.DayAfterNotificationSent))
                )
            )
            .Select(t => new TaskWithNotificationDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status.Name,
                DueDate = t.DueDate,
                AssignedToUser = t.AssignedTo != null ? t.AssignedTo.EmailAddress : null,
                Notification = new TaskNotificationDto
                {
                    Id = t.TaskNotifications.First().Id,
                    DueDateNotificationSent = t.TaskNotifications.First().DueDateNotificationSent,
                    DayAfterNotificationSent = t.TaskNotifications.First().DayAfterNotificationSent,
                    LastNotificationDate = t.TaskNotifications.First().LastNotificationDate
                }
            })
            .ToListAsync();

        Console.WriteLine($"Found {tasksToNotify.Count} tasks to notify.");

        return tasksToNotify;
    }


    public async Task UnassignUserFromTaskAsync(int taskId, int userId)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId)
                    ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        if (task.IsArchived)
        {
            throw new InvalidOperationException($"Task with id {taskId} is archived and cannot be modified.");
        }

        if (task.AssignedToId != userId)
        {
            throw new InvalidOperationException($"User with id {userId} is not assigned to task with id {taskId}.");
        }

        task.AssignedToId = null;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTaskNotificationsAsync()
    {
        // Get the current UTC date (without time component)
        var currentUtcDate = DateTime.UtcNow.Date;

        // Calculate yesterday's date
        var yesterdayUtcDate = currentUtcDate.AddDays(-1);

        // Retrieve TaskNotifications needing updates
        var taskNotificationsToUpdate = await _dbContext.TaskNotifications
            .Include(tn => tn.Task)
            .Where(tn =>
                tn.Task.DueDate.HasValue &&
                !tn.Task.IsArchived &&
                (
                    (tn.Task.DueDate.Value.Date == currentUtcDate && !tn.DueDateNotificationSent) ||
                    (tn.Task.DueDate.Value.Date == yesterdayUtcDate && !tn.DayAfterNotificationSent)
                )
            )
            .ToListAsync();

        foreach (var notification in taskNotificationsToUpdate)
        {
            if (notification.Task.DueDate.Value.Date == currentUtcDate && !notification.DueDateNotificationSent)
            {
                notification.DueDateNotificationSent = true;
            }

            if (notification.Task.DueDate.Value.Date == yesterdayUtcDate && !notification.DayAfterNotificationSent)
            {
                notification.DayAfterNotificationSent = true;
            }

            notification.LastNotificationDate = DateTime.UtcNow;
        }

        // Persist changes to the database
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTaskPriorityAsync(int taskId, PriorityInputModel inputModel)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId)
                    ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        if (task.IsArchived)
        {
            throw new InvalidOperationException($"Task with id {taskId} is archived and cannot be updated.");
        }

        task.PriorityId = inputModel.PriorityId;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTaskStatusAsync(int taskId, StatusInputModel inputModel)
    {
        var task = await _dbContext.Tasks.FindAsync(taskId)
                    ?? throw new ResourceNotFoundException($"Task with id {taskId} not found");

        if (task.IsArchived)
        {
            throw new InvalidOperationException($"Task with id {taskId} is archived and cannot be updated.");
        }

        task.StatusId = inputModel.StatusId;

        await _dbContext.SaveChangesAsync();
    }

}