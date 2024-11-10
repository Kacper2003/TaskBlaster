namespace TaskBlaster.TaskManagement.DAL.Entities;

public class TaskNotification
{
    public int Id { get; set; }

    // Foreign Key
    public int TaskId { get; set; } // Not Null
    public Task Task { get; set; } = null!;

    public bool DueDateNotificationSent { get; set; } // Not Null
    public bool DayAfterNotificationSent { get; set; } // Not Null
    public DateTime? LastNotificationDate { get; set; } // Nullable
}

