using System.Text.Json;

namespace TaskBlaster.TaskManagement.Models;

public class ExceptionModel
{
    public int StatusCode { get; set; }
    public string ExceptionMessage { get; set; } = "";
    public override string ToString() => JsonSerializer.Serialize(this);
}