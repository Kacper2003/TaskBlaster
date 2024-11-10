using System.ComponentModel.DataAnnotations;

namespace TaskBlaster.TaskManagement.Models.InputModels;

public class CommentInputModel
{
    [Required(ErrorMessage = "Content is required.")]
    [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
    public string ContentAsMarkdown { get; set; } = "";
}
