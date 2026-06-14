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
    public class BatchProcessingTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BatchProcessingTests(CustomWebApplicationFactory factory)
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

        private async Task AuthenticateAsBankOfficerAsync()
        {
            var loginCmd = new LoginCommand("bank@ccms.local", "Password@123");
            var loginRes = await _client.PostAsJsonAsync("/auth/login", loginCmd);
            var authResult = await loginRes.Content.ReadFromJsonAsync<LoginResult>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult!.Token);
        }

        private MultipartFormDataContent CreateValidCaseContent(string accountNumber, string aadhaar, string pan)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("John Doe"), "ComplainantName");
            content.Add(new StringContent("Jane Defendant"), "DefendantName");
            content.Add(new StringContent(aadhaar), "AadhaarNumber");
            content.Add(new StringContent(pan), "PanNumber");
            content.Add(new StringContent(accountNumber), "AccountNumber");
            content.Add(new StringContent("Test Bank"), "BankName");
            content.Add(new StringContent("1"), "OrderType"); // Balance Enquiry
            
            var dummyPdf = new byte[] { 1 };
            
            var courtFileContent = new ByteArrayContent(dummyPdf);
            courtFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            content.Add(courtFileContent, "CourtOrderFile", "court.pdf");

            var aadhaarFileContent = new ByteArrayContent(dummyPdf);
            aadhaarFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            content.Add(aadhaarFileContent, "AadhaarCopyFile", "aadhaar.pdf");

            var panFileContent = new ByteArrayContent(dummyPdf);
            panFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            content.Add(panFileContent, "PanCopyFile", "pan.pdf");

            return content;
        }

        [Fact]
        public async Task TriggerBatch_WithValidAccount_SetsStatusToAccountValidated()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("1234567890", "123456789012", "ABCDE1234F"); // Matches seeded bank customer
            var response = await _client.PostAsync("/api/court/cases", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!responseBody.Contains("\"id\"") && !responseBody.Contains("\"Id\"")) throw new System.Exception($"JSON IS: {responseBody}");
            var jsonDoc = JsonDocument.Parse(responseBody);
            var caseId = jsonDoc.RootElement.TryGetProperty("id", out var idProp) ? idProp.GetString() : jsonDoc.RootElement.GetProperty("Id").GetString();

            // Act - Trigger batch
            var batchResponse = await _client.PostAsync("/api/batch/run", null);
            batchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            var caseResponse = await _client.GetAsync($"/api/court/cases/{caseId}");
            var caseData = await caseResponse.Content.ReadAsStringAsync();
            caseData.Should().Contain("\"status\":\"AccountValidated\"");
        }

        [Fact]
        public async Task TriggerBatch_WithInvalidAccount_SetsStatusToAccountNotFound()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("9999999999", "999999999999", "XXXXX9999X"); // Doesn't match
            var response = await _client.PostAsync("/api/court/cases", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var caseId = jsonDoc.RootElement.GetProperty("id").GetString();

            // Act - Trigger batch
            var batchResponse = await _client.PostAsync("/api/batch/run", null);
            batchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            var caseResponse = await _client.GetAsync($"/api/court/cases/{caseId}");
            var caseData = await caseResponse.Content.ReadAsStringAsync();
            caseData.Should().Contain("\"status\":\"AccountNotFound\"");
        }

        private class LoginResult
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
