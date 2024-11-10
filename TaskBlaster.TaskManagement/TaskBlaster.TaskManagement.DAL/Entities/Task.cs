namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsArchived { get; set; } = false; // Property for archiving tasks

    // Foreign Keys
    public int PriorityId { get; set; }
    public Priority Priority { get; set; } = null!;

    public int StatusId { get; set; }
    public Status Status { get; set; } = null!;

    public int? AssignedToId { get; set; } // Nullable
    public User? AssignedTo { get; set; } // Nullable navigation property

    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    // Navigation properties
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public ICollection<TaskNotification> TaskNotifications { get; set; } = new List<TaskNotification>();
}
