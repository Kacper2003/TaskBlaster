namespace TaskBlaster.TaskManagement.DAL.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!; // Not Null
    public string EmailAddress { get; set; } = null!; // Not Null
    public string? ProfileImageUrl { get; set; } // Nullable
    public DateTime CreatedAt { get; set; } // Not Null

    public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
    public ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
}
