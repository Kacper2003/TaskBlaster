namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Author { get; set; } = null!; // Not Null
    public string ContentAsMarkdown { get; set; } = null!; // Not Null
    public DateTime CreatedDate { get; set; } // Not Null

    // Foreign Key
    public int TaskId { get; set; } // Not Null
    public Task Task { get; set; } = null!;
}
