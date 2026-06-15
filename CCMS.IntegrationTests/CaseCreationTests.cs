using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCMS.Application.Features.Auth.Commands;
using FluentAssertions;
using Xunit;

namespace CCMS.IntegrationTests
{
    public class CaseCreationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CaseCreationTests(CustomWebApplicationFactory factory)
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

        private MultipartFormDataContent CreateValidCaseContent(string orderType, string freezeAmount = "")
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("John Doe"), "ComplainantName");
            content.Add(new StringContent("Jane Defendant"), "DefendantName");
            content.Add(new StringContent("123456789012"), "AadhaarNumber");
            content.Add(new StringContent("ABCDE1234F"), "PanNumber");
            content.Add(new StringContent("1234567890"), "AccountNumber");
            content.Add(new StringContent("Test Bank"), "BankName");
            content.Add(new StringContent(orderType), "OrderType");
            
            if (!string.IsNullOrEmpty(freezeAmount))
            {
                content.Add(new StringContent(freezeAmount), "FreezeAmount");
            }

            var dummyPdf = new byte[] { 1, 2, 3, 4 };
            
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
        public async Task CreateCase_FreezeOrder_Returns200AndCaseNumber()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("0", "5000");

            // Act
            var response = await _client.PostAsync("/api/court/cases", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("caseNumber");
            responseBody.Should().Contain("CCMS-");
        }

        [Fact]
        public async Task CreateCase_BalanceEnquiry_IgnoresFreezeAmount_Returns200()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("1"); // 1 is Balance Enquiry

            // Act
            var response = await _client.PostAsync("/api/court/cases", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateCase_MissingFile_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("1");
            
            // Remove the PAN copy file to trigger validation error
            var panField = content.GetEnumerator();
            // Since we can't easily remove a specific part from MultipartFormDataContent natively without iterating
            // Let's just create a new one missing the file
            var badContent = new MultipartFormDataContent();
            badContent.Add(new StringContent("John Doe"), "ComplainantName");
            badContent.Add(new StringContent("Jane Defendant"), "DefendantName");
            badContent.Add(new StringContent("123456789012"), "AadhaarNumber");
            badContent.Add(new StringContent("ABCDE1234F"), "PanNumber");
            badContent.Add(new StringContent("1234567890"), "AccountNumber");
            badContent.Add(new StringContent("Test Bank"), "BankName");
            badContent.Add(new StringContent("1"), "OrderType");
            
            var dummyPdf = new byte[] { 1, 2, 3, 4 };
            var courtFileContent = new ByteArrayContent(dummyPdf);
            courtFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            badContent.Add(courtFileContent, "CourtOrderFile", "court.pdf");

            // Deliberately missing Aadhaar and PAN files

            // Act
            var response = await _client.PostAsync("/api/court/cases", badContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCase_FreezeOrderMissingAmount_ReturnsBadRequest()
        {
            // Arrange
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("0"); // Freeze order but no amount

            // Act
            var response = await _client.PostAsync("/api/court/cases", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Freeze Amount is required");
        }

        private class LoginResult
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
