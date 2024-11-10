using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using TaskBlaster.TaskManagement.API.Services.Interfaces;

namespace TaskBlaster.TaskManagement.API.Services.Implementations;

public class M2MAuthenticationService : IM2MAuthenticationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public M2MAuthenticationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Modular method that takes in a config key for the correct audience
    public async Task<string?> RetrieveAccessTokenAsync()
    {
        // Retrieve the relevant Auth0 configurations using the audienceKey
        var domain = _configuration["Auth0:Domain"];
        var clientId = _configuration["Auth0:ClientId"];
        var clientSecret = _configuration["Auth0:ClientSecret"];
        var audience = _configuration[$"Auth0:NotificationApiAudience"];

        var tokenEndpoint = $"{domain}/oauth/token";

        var requestBody = new
        {
            grant_type = "client_credentials",
            client_id = clientId,
            client_secret = clientSecret,
            audience = audience
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(tokenEndpoint, requestContent);

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);

        return tokenResponse?.AccessToken;
    }

    // Map properties from the token response
    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}