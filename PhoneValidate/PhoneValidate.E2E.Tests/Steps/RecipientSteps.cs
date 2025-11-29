using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TechTalk.SpecFlow;

namespace PhoneValidationE2E.Tests.StepDefinitions
{
    [Binding]
    public class RecipientSteps : IDisposable
    {
        private HttpClient _client = null!;
        private string _jwtToken = string.Empty;
        private HttpResponseMessage _response = null!;
        private string _createdRecipientId = string.Empty;
        private string _lastGeneratedPhoneNumber = string.Empty;

        private readonly string _baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
            ?? "http://localhost:50000";

        [Given(@"the API is running")]
        public void GivenTheAPIIsRunning()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        [When(@"I login with username ""(.*)"" and password ""(.*)""")]
        public async Task WhenILoginWithUsernameAndPassword(string username, string password)
        {
            var loginRequest = new
            {
                username,
                password
            };

            _response = await _client.PostAsJsonAsync("/api/auth/token", loginRequest);
        }

        [Then(@"I should receive a valid JWT token")]
        public async Task ThenIShouldReceiveAValidJWTToken()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await _response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);

            _jwtToken = jsonDoc.RootElement.GetProperty("token").GetString()!;
            _jwtToken.Should().NotBeNullOrEmpty();

            // Adiciona o token no header para próximas requisições
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
        }

        [When(@"I call GET ""(.*)"" without authentication")]
        public async Task WhenICallGETWithoutAuthentication(string endpoint)
        {
            _response = await _client.GetAsync(endpoint);
        }

        [When(@"I call GET ""(.*)"" with authentication")]
        public async Task WhenICallGETWithAuthentication(string endpoint)
        {
            _response = await _client.GetAsync(endpoint);
        }

        [When(@"I call GET ""(.*)"" with ""(.*)"" equals to ""(.*)""")]
        public async Task WhenICallGETWithAuthenticationAndQueryParameter(string endpoint, string paramName, string paramValue)
        {
            var uri = $"{endpoint}?{paramName}={Uri.EscapeDataString(paramValue)}";
            _response = await _client.GetAsync(uri);
        }

        [Then(@"I should receive status code (.*)")]
        public void ThenIShouldReceiveStatusCode(int expectedStatusCode)
        {
            ((int)_response.StatusCode).Should().Be(expectedStatusCode);
        }

        [Then(@"the response should contain ""(.*)""")]
        public async Task ThenTheResponseShouldContain(string expectedText)
        {
            var content = await _response.Content.ReadAsStringAsync();
            content.Should().Contain(expectedText);
        }

        [Then(@"the response should contain recipient data")]
        public async Task ThenTheResponseShouldContainRecipientData()
        {
            var content = await _response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(content);

            jsonDoc.RootElement.TryGetProperty("id", out _).Should().BeTrue();
            jsonDoc.RootElement.TryGetProperty("updatedAt", out _).Should().BeTrue();
            jsonDoc.RootElement.TryGetProperty("phoneNumber", out _).Should().BeTrue();
        }

        [When(@"I create a recipient with phone number ""(.*)""")]
        public async Task WhenICreateARecipientWithPhoneNumber(string phoneNumber)
        {
            var createRequest = new
            {
                phoneNumber
            };

            _response = await _client.PostAsJsonAsync("/Recipient", createRequest);
        }

        [When(@"I create a recipient with a unique phone number")]
        public async Task WhenICreateARecipientWithAUniquePhoneNumber()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var random = new Random().Next(1000, 9999);
            _lastGeneratedPhoneNumber = $"+55119{timestamp % 100000000}{random}";

            var createRequest = new
            {
                phoneNumber = _lastGeneratedPhoneNumber
            };

            _response = await _client.PostAsJsonAsync("/Recipient", createRequest);
        }

        [When(@"I create a recipient with phone number ""(.*)"" without authentication")]
        public async Task WhenICreateARecipientWithPhoneNumberWithoutAuthentication(string phoneNumber)
        {
            // Limpar o header de autenticação temporariamente
            var previousAuth = _client.DefaultRequestHeaders.Authorization;
            _client.DefaultRequestHeaders.Authorization = null;

            var createRequest = new
            {
                phoneNumber
            };

            _response = await _client.PostAsJsonAsync("/Recipient", createRequest);

            // Restaurar o header de autenticação
            _client.DefaultRequestHeaders.Authorization = previousAuth;
        }

        [Then(@"the response should contain the created recipient")]
        public async Task ThenTheResponseShouldContainTheCreatedRecipient()
        {
            var content = await _response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);

            jsonDoc.RootElement.TryGetProperty("id", out var idElement).Should().BeTrue();
            jsonDoc.RootElement.TryGetProperty("phoneNumber", out _).Should().BeTrue();
            jsonDoc.RootElement.TryGetProperty("updatedAt", out _).Should().BeTrue();

            _createdRecipientId = idElement.GetGuid().ToString();
            _createdRecipientId.Should().NotBeNullOrEmpty();
        }

        public void Dispose()
        {
            _response?.Dispose();
            _client?.Dispose();
        }
    }
}