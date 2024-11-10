namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Priority
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // Not Null
    public string? Description { get; set; } // Nullable

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
