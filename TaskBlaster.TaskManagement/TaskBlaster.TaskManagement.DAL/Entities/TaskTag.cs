namespace TaskBlaster.TaskManagement.DAL.Entities;

public class TaskTag
{
    // Composite Key of TaskId and TagId
    public int TaskId { get; set; } // Not Null
    public Task Task { get; set; } = null!;

    public int TagId { get; set; } // Not Null
    public Tag Tag { get; set; } = null!;
}
