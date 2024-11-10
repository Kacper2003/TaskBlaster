namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Status
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 

    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
