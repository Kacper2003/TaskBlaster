namespace TaskBlaster.TaskManagement.DAL.Entities;

public class TaskTag
{
    // Composite Key of TaskId and TagId
    public int TaskId { get; set; }
    public Task Task { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
