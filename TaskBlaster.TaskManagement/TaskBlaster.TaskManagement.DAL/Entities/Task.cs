namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = null!; // Not Null
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } // Not Null
    public DateTime? DueDate { get; set; } // Nullable
    public bool IsArchived { get; set; } = false; // Property for archiving tasks

    // Foreign Keys
    public int PriorityId { get; set; } // Not Null
    public Priority Priority { get; set; } = null!;

    public int StatusId { get; set; } // Not Null
    public Status Status { get; set; } = null!;

    public int? AssignedToId { get; set; } // Nullable
    public User? AssignedTo { get; set; } // Nullable navigation property

    public int CreatedById { get; set; } // Not Null
    public User CreatedBy { get; set; } = null!;

    // Navigation properties
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public ICollection<TaskNotification> TaskNotifications { get; set; } = new List<TaskNotification>();
}
