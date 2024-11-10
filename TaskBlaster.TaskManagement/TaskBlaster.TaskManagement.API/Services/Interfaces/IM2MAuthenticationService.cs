namespace TaskBlaster.TaskManagement.API.Services.Interfaces;

public interface IM2MAuthenticationService
{
    Task<string?> RetrieveAccessTokenAsync();
}