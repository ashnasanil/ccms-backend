using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCMS.Application.Features.Auth.Commands;
using FluentAssertions;
using Xunit;

namespace CCMS.IntegrationTests
{
    public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthenticationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCourtOfficerCredentials_ReturnsToken()
        {
            // Arrange
            var command = new LoginCommand("court@ccms.local", "Password@123");

            // Act
            var response = await _client.PostAsJsonAsync("/auth/login", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("token");
        }

        [Fact]
        public async Task Login_WithValidBankOfficerCredentials_ReturnsToken()
        {
            // Arrange
            var command = new LoginCommand("bank@ccms.local", "Password@123");

            // Act
            var response = await _client.PostAsJsonAsync("/auth/login", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("token");
        }

        [Fact]
        public async Task CourtOfficer_AccessingBankEndpoint_ReturnsForbidden()
        {
            // Arrange
            var loginCmd = new LoginCommand("court@ccms.local", "Password@123");
            var loginRes = await _client.PostAsJsonAsync("/auth/login", loginCmd);
            var authResult = await loginRes.Content.ReadFromJsonAsync<LoginResult>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult!.Token);

            // Act
            var response = await _client.GetAsync("/api/bank/cases?statusCategory=Pending");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task BankOfficer_AccessingCourtEndpoint_ReturnsForbidden()
        {
            // Arrange
            var loginCmd = new LoginCommand("bank@ccms.local", "Password@123");
            var loginRes = await _client.PostAsJsonAsync("/auth/login", loginCmd);
            var authResult = await loginRes.Content.ReadFromJsonAsync<LoginResult>();
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult!.Token);

            // Act
            var response = await _client.PostAsync("/api/court/cases", new MultipartFormDataContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        private class LoginResult
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
