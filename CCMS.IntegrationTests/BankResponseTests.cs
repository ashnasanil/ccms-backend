using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CCMS.Application.Features.Auth.Commands;
using CCMS.Application.Features.Bank.Responses.Commands;
using FluentAssertions;
using Xunit;
using System.Text.Json;

namespace CCMS.IntegrationTests
{
    public class BankResponseTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BankResponseTests(CustomWebApplicationFactory factory)
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
        public async Task SubmitFreezeResponse_ValidRequest_SetsStatusToFreezeApplied()
        {
            // 1. Create Case
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("0", "5000"); // Freeze
            var response = await _client.PostAsync("/api/court/cases", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var caseIdStr = jsonDoc.RootElement.GetProperty("id").GetString();
            var caseId = System.Guid.Parse(caseIdStr!);

            // 2. Trigger batch
            await _client.PostAsync("/api/batch/run", null);

            // 3. Login as Bank Officer and submit freeze response
            await AuthenticateAsBankOfficerAsync();
            var freezeDto = new CCMS.API.Controllers.BankResponsesController.SubmitFreezeResponseDto(5000, "Account frozen");

            var bankResponse = await _client.PostAsJsonAsync($"/api/bank/cases/{caseId}/freeze", freezeDto);
            bankResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // 4. Verify Court sees it as FreezeApplied
            await AuthenticateAsCourtOfficerAsync();
            var caseResponse = await _client.GetAsync($"/api/court/cases/{caseId}");
            var caseData = await caseResponse.Content.ReadAsStringAsync();
            caseData.Should().Contain("\"status\":\"FreezeApplied\"");
        }

        [Fact]
        public async Task SubmitBalanceResponse_ValidRequest_SetsStatusToBalanceProvided()
        {
            // 1. Create Case
            await AuthenticateAsCourtOfficerAsync();
            var content = CreateValidCaseContent("1"); // Balance
            var response = await _client.PostAsync("/api/court/cases", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var caseIdStr = jsonDoc.RootElement.GetProperty("id").GetString();
            var caseId = System.Guid.Parse(caseIdStr!);

            // 2. Trigger batch
            await _client.PostAsync("/api/batch/run", null);

            // 3. Login as Bank Officer and submit balance response
            await AuthenticateAsBankOfficerAsync();
            var balanceDto = new CCMS.API.Controllers.BankResponsesController.SubmitBalanceResponseDto(50000, "Balance provided");

            var bankResponse = await _client.PostAsJsonAsync($"/api/bank/cases/{caseId}/balance", balanceDto);
            bankResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // 4. Verify Court sees it as BalanceProvided
            await AuthenticateAsCourtOfficerAsync();
            var caseResponse = await _client.GetAsync($"/api/court/cases/{caseId}");
            var caseData = await caseResponse.Content.ReadAsStringAsync();
            caseData.Should().Contain("\"status\":\"BalanceProvided\"");
        }

        private class LoginResult
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
