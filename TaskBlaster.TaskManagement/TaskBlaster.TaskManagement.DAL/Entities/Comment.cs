namespace TaskBlaster.TaskManagement.DAL.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Author { get; set; } = null!;
    public string ContentAsMarkdown { get; set; } = null!;
    public DateTime CreatedDate { get; set; }

    // Foreign Key
    public int TaskId { get; set; }
    public Task Task { get; set; } = null!;
}
