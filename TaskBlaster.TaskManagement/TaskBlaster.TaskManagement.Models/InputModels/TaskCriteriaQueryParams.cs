namespace TaskBlaster.TaskManagement.Models.InputModels;

public class TaskCriteriaQueryParams
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public string? SearchValue { get; set; }
}
