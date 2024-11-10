using System.ComponentModel.DataAnnotations;

namespace TaskBlaster.TaskManagement.Models.InputModels;

public class PriorityInputModel
{
    [Range(1, 4, ErrorMessage = "PriorityId must be between 1 and 4.")]
    public int PriorityId { get; set; }
}
