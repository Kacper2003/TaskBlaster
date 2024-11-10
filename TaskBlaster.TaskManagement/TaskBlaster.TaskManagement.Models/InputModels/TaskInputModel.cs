using System.ComponentModel.DataAnnotations;

namespace TaskBlaster.TaskManagement.Models.InputModels;

public class TaskInputModel
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = "";

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Range(1, 5, ErrorMessage = "StatusId must be between 1 and 5.")]
    public int StatusId { get; set; }

    [Range(1, 4, ErrorMessage = "PriorityId must be between 1 and 4.")]
    public int PriorityId { get; set; }

    [DataType(DataType.Date, ErrorMessage = "DueDate must be a valid date.")]
    public DateTime? DueDate { get; set; }

    [EmailAddress(ErrorMessage = "AssignedToUser must be a valid email address.")]
    public string? AssignedToUser { get; set; }
}
