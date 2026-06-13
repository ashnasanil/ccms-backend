using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Application.DTOs.Court;
using CCMS.Application.Helpers;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;

namespace CCMS.Application.Services
{
    public class CourtService : ICourtService
    {
        public Task<CourtDashboardDto> GetDashboardAsync()
        {
            // TODO: Fetch dashboard statistics from the database
            return Task.FromResult(new CourtDashboardDto());
        }

        public Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto dto)
        {
            // Validate Documents
            if (dto.Documents == null || dto.Documents.Count != 3)
            {
                throw new ArgumentException("Exactly 3 documents are required (Court Order, Aadhaar Copy, PAN Copy).");
            }

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var maxFileSize = 5 * 1024 * 1024; // 5 MB

            foreach (var doc in dto.Documents)
            {
                if (doc.Length > maxFileSize)
                {
                    throw new ArgumentException($"File {doc.FileName} exceeds the maximum size of 5MB.");
                }

                var extension = Path.GetExtension(doc.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    throw new ArgumentException($"File {doc.FileName} has an invalid format. Supported formats: PDF, JPG, JPEG, PNG. Rejected formats like EXE, BAT, JS, ZIP are not allowed.");
                }
            }

            // Validate Freeze Amount if FreezeAccount
            if (dto.OrderType == OrderType.FreezeAccount && (!dto.FreezeAmount.HasValue || dto.FreezeAmount.Value <= 0))
            {
                throw new ArgumentException("Freeze Amount is required for Freeze Account orders.");
            }

            // Generate Case Number
            var caseNumber = CaseNumberGenerator.Generate();
            var createdDate = DateTime.UtcNow;

            // Create Case Object (stubbed for architecture)
            var newCase = new Case
            {
                CaseNumber = caseNumber,
                ComplainantName = dto.ComplainantName,
                DefendantName = dto.DefendantName,
                AadhaarNumber = dto.AadhaarNumber,
                PanNumber = dto.PanNumber,
                AccountNumber = dto.AccountNumber,
                BankName = dto.BankName,
                OrderType = dto.OrderType,
                FreezeAmount = dto.FreezeAmount,
                Status = CaseStatus.Pending,
                CreatedDate = createdDate
            };

            // Return Response
            return Task.FromResult(new CaseResponseDto
            {
                CaseNumber = newCase.CaseNumber,
                Status = newCase.Status,
                CreatedDate = newCase.CreatedDate
            });
        }
    }
}
