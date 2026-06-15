using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCMS.Application.Features.Auth.Commands;
using FluentAssertions;
using Xunit;
using System.Text.Json;

namespace CCMS.IntegrationTests
{
    public class DashboardTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public DashboardTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task AuthenticateAsCourtOfficerAsync()
        {
            var loginCmd = new LoginCommand("court@ccms.local", "Password@123");
            var loginRes = await _client.PostAsJsonAsync("/auth/login", loginCmd);
            var authResult = await loginRes.Content.ReadFromJsonAsync<LoginResult>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult!.Token);
        }

        [Fact]
        public async Task GetDashboard_ReturnsValidAggregates()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();

            // Act
            var response = await _client.GetAsync("/api/court/dashboard");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            
            var totalCases = jsonDoc.RootElement.GetProperty("totalCases").GetInt32();
            var pendingCases = jsonDoc.RootElement.GetProperty("pendingCases").GetInt32();
            var validatedCases = jsonDoc.RootElement.GetProperty("accountValidatedCases").GetInt32();
            
            // Total should be >= 2 because of seeded data in CustomWebApplicationFactory
            totalCases.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetCaseById_MasksSensitiveInformation()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            
            // Get the first seeded case
            var seededCaseId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

            // Act
            var response = await _client.GetAsync($"/api/court/cases/{seededCaseId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            
            var aadhaar = jsonDoc.RootElement.GetProperty("aadhaarNumber").GetString();
            var pan = jsonDoc.RootElement.GetProperty("panNumber").GetString();
            var account = jsonDoc.RootElement.GetProperty("accountNumber").GetString();
            
            // The unmasked values in seed data are: 123456789012, ABCDE1234F, 1234567890
            aadhaar.Should().Contain("*");
            pan.Should().Contain("*");
            account.Should().Contain("*");
        }

        private class LoginResult
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
