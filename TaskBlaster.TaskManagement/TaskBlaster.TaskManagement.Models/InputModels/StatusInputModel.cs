using System.ComponentModel.DataAnnotations;

namespace TaskBlaster.TaskManagement.Models.InputModels;

public class StatusInputModel
{
    [Range(1, 5, ErrorMessage = "StatusId must be between 1 and 5.")]
    public int StatusId { get; set; }
}
