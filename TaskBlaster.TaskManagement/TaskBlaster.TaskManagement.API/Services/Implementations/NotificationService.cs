using TaskBlaster.TaskManagement.API.Services.Interfaces;
using TaskBlaster.TaskManagement.DAL.Interfaces;

namespace TaskBlaster.TaskManagement.API.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;

    public NotificationService(IHttpClientFactory httpClientFactory, IUserRepository userRepository, ITaskRepository taskRepository)
    {
        _httpClientFactory = httpClientFactory;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
    }

    public async Task SendAssignedNotification(int userId, int taskId)
    {
        // Retrieve the user and task details
        var user = await _userRepository.GetUserByIdAsync(userId);
        var taskTitle = await _taskRepository.GetTaskTitleByIdAsync(taskId);

        if (user == null || taskTitle == null)
        {
            throw new ArgumentException("User or Task not found");
        }

        var client = _httpClientFactory.CreateClient("NotificationApiClient");

        var requestBody = new
        {
            To = user.EmailAddress,
            Subject = "You have been assigned a new task",
            IsHtml = true,
            Content = $@"
                <h1>New Task Assigned</h1>
                <p>Dear {user.FullName},</p>
                <p>You have been assigned a new task: <strong>{taskTitle}</strong>.</p>
                <p>Best regards,<br/>Task Blaster Team</p>
            "};

        // Send a POST request to the Notification API for sending assigned notification
        var response = await client.PostAsJsonAsync("emails/basic", requestBody);

        // Handle response and ensure the request was successful
        response.EnsureSuccessStatusCode();
    }

    public async Task SendUnassignedNotification(int userId, int taskId)
    {
        // Retrieve the user and task details
        var user = await _userRepository.GetUserByIdAsync(userId);
        var taskTitle = await _taskRepository.GetTaskTitleByIdAsync(taskId);

        if (user == null || taskTitle == null)
        {
            throw new ArgumentException("User or Task not found");
        }

        var client = _httpClientFactory.CreateClient("NotificationApiClient");

        var requestBody = new
        {
            To = user.EmailAddress,
            Subject = "You have been unassigned from a task",
            IsHtml = true,
            Content = $@"
                <h1>Task Unassigned</h1>
                <p>Dear {user.FullName},</p>
                <p>You have been unassigned from the task: <strong>{taskTitle}</strong>.</p>
                <p>Best regards,<br/>Task Blaster Team</p>
            "};

        // Send a POST request to the Notification API for sending unassigned notification
        var response = await client.PostAsJsonAsync("emails/basic", requestBody);

        // Handle response and ensure the request was successful
        response.EnsureSuccessStatusCode();
    }
}