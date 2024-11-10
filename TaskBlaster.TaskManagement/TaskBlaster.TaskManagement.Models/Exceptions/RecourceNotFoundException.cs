namespace TaskBlaster.TaskManagement.Models.Exceptions;

public class ResourceNotFoundException : Exception
{  
    // * Constructor with no parameters
    public ResourceNotFoundException() { }

    // * Constructor with a string parameter
    public ResourceNotFoundException(string message) : base(message) { }

    // * Constructor with a string and an Exception parameter
    public ResourceNotFoundException(string message, Exception inner) : base(message, inner) { }
}