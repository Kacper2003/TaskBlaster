namespace TaskBlaster.TaskManagement.DAL.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string? ProfileImageUrl { get; set; } 
    public DateTime CreatedAt { get; set; }

    public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
    public ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
}
