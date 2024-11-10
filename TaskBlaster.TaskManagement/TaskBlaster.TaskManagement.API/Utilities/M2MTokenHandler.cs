using TaskBlaster.TaskManagement.API.Services.Interfaces;

namespace TaskBlaster.TaskManagement.API.Utilities;

public class M2MTokenHandler : DelegatingHandler
{
    private readonly IM2MAuthenticationService _authService;
    public M2MTokenHandler(IM2MAuthenticationService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get the access token
        var accessToken = await _authService.RetrieveAccessTokenAsync();

        // Add the Bearer token to the request headers
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}

