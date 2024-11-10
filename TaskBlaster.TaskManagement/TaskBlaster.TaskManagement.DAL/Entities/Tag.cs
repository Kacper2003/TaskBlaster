namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 
    
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
