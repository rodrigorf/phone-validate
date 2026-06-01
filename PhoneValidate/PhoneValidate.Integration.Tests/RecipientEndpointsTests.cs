using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PhoneValidate.Integration.Tests;

public class RecipientEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RecipientEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/healthystatus");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", body);
    }

    [Fact]
    public async Task AuthToken_WithValidCredentials_ReturnsToken()
    {
        var token = await GetTokenAsync();

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task AuthToken_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token",
            new { username = "wrong", password = "wrong" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostRecipient_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/Recipient",
            new { phoneNumber = "+55 11 90000-0001" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostThenGetRecipient_WithToken_ReturnsCreatedRecipient()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create
        var createResponse = await _client.PostAsJsonAsync("/Recipient",
            new { phoneNumber = "+55 11 98888-7777" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var phoneNumber = created.GetProperty("phoneNumber").GetString();
        Assert.Equal("+5511988887777", phoneNumber); // normalized

        // Get
        var getResponse = await _client.GetAsync($"/Recipient?phoneNumber={Uri.EscapeDataString(phoneNumber!)}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(phoneNumber, fetched.GetProperty("phoneNumber").GetString());
    }

    private async Task<string> GetTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token",
            new { username = "test", password = "test" });
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("token").GetString()!;
    }
}
