namespace TaskBlaster.TaskManagement.Notifications.Models;

public class BasicEmailInputModel
{
    public string To { get; set; } = "";
    public string Subject { get; set; } = "";
    public bool IsHtml { get; set; }
    public string Content { get; set; } = "";
}
