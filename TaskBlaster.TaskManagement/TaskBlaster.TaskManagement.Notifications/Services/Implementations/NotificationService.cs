using TaskBlaster.TaskManagement.Notifications.Services.Interfaces;
using TaskBlaster.TaskManagement.Notifications.Models;
using TaskBlaster.TaskManagement.Models.Dtos;

namespace TaskBlaster.TaskManagement.Notifications.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IMailService _mailService;
    private readonly ITaskService _taskService;

    public NotificationService(IMailService mailService, ITaskService taskService)
    {
        _mailService = mailService;
        _taskService = taskService;
    }

    public async Task SendDailyReports()
    {
        var tasks = await _taskService.GetTasksForNotifications();

        // Current UTC date without time
        DateTime today = DateTime.UtcNow.Date;
        DateTime yesterday = today.AddDays(-1);

        foreach (var task in tasks)
        {
            DateTime taskDueDate = task.DueDate?.Date ?? DateTime.MinValue;

            string subject;
            string content;

            if (taskDueDate == today && !task.Notification.DueDateNotificationSent)
            {
                subject = $"Task Due Today: {task.Title}";
                content = GenerateEmailContent(task, "today");
            }
            else if (taskDueDate == yesterday && !task.Notification.DayAfterNotificationSent)
            {
                subject = $"Task Was Due Yesterday: {task.Title}";
                content = GenerateEmailContent(task, "yesterday");
            }
            else
            {
                // Skip tasks that don't meet the criteria
                continue;
            }

            try
            {
                await _mailService.SendBasicEmailAsync(
                    to: task.AssignedToUser,
                    subject: subject,
                    content: content,
                    contentType: EmailContentType.Html
                );

                Console.WriteLine($"Email sent to {task.AssignedToUser} for Task ID {task.Id}");

                // Update the notification flags based on due date
                if (taskDueDate == today)
                {
                    task.Notification.DueDateNotificationSent = true;
                }
                else if (taskDueDate == yesterday)
                {
                    task.Notification.DayAfterNotificationSent = true;
                }
                
                task.Notification.LastNotificationDate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {task.AssignedToUser} for Task ID {task.Id}: {ex.Message}");
                // Optionally, log the error using a logging framework
            }
        }

        await _taskService.UpdateTaskNotifications();
    }

    private string GenerateEmailContent(TaskWithNotificationDto task, string timing)
    {
        if (timing == "today")
        {
            return $@"
                <h1>Task Due Today</h1>
                <p>Dear User,</p>
                <p>Your task <strong>{task.Title}</strong> is due today ({task.DueDate?.ToString("MMMM dd, yyyy")}).</p>
                <p>Please ensure to complete it on time.</p>
                <p>Best regards,<br/>Task Blaster Team</p>
            ";
        }
        if (timing == "yesterday")
        {
            return $@"
                <h1>Task Was Due Yesterday</h1>
                <p>Dear User,</p>
                <p>Your task <strong>{task.Title}</strong> was due yesterday ({task.DueDate?.ToString("MMMM dd, yyyy")}).</p>
                <p>Please address this task as soon as possible.</p>
                <p>Best regards,<br/>Task Blaster Team</p>
            ";
        }
        
        return string.Empty;
    }
}

