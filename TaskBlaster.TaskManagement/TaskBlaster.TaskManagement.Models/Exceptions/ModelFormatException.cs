namespace TaskBlaster.TaskManagement.Models.Exceptions;

public class ModelFormatException : Exception
{
    public ModelFormatException() { }

    public ModelFormatException(string message) : base(message) { }
    
    public ModelFormatException(string message, Exception inner) : base(message, inner) { }
}