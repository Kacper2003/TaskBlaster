namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // Not Null
    public string? Description { get; set; } // Nullable

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
