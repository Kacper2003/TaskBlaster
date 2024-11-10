namespace TaskBlaster.TaskManagement.Notifications.Models;

public class TemplateEmailInputModel
{
    public string To { get; set; } = ""; 
    public string Subject { get; set; } = "";
    public int TemplateId { get; set; } 
    public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>(); 
}
