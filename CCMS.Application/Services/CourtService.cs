using System;
using System.Collections.Generic;
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
        private readonly ICaseRepository _caseRepository;

        public CourtService(ICaseRepository caseRepository)
        {
            _caseRepository = caseRepository;
        }

        public async Task<CourtDashboardDto> GetDashboardAsync()
        {
            return new CourtDashboardDto
            {
                TotalCases = await _caseRepository.GetTotalCountAsync(),
                PendingCases = await _caseRepository.GetCountByStatusAsync(CaseStatus.Pending),
                AccountValidatedCases = await _caseRepository.GetCountByStatusAsync(CaseStatus.AccountValidated),
                AccountNotFoundCases = await _caseRepository.GetCountByStatusAsync(CaseStatus.AccountNotFound),
                FreezeAppliedCases = await _caseRepository.GetCountByOrderTypeAsync(OrderType.FreezeAccount),
                BalanceProvidedCases = await _caseRepository.GetCountByOrderTypeAsync(OrderType.ProvideBalance)
            };
        }

        public async Task<IEnumerable<CaseListDto>> GetCasesAsync()
        {
            var cases = await _caseRepository.GetAllAsync();
            return cases.Select(c => new CaseListDto
            {
                CaseNumber = c.CaseNumber,
                DefendantName = c.DefendantName,
                OrderType = c.OrderType.ToString(), // Or map properly
                Status = c.Status.ToString(),
                CreatedDate = c.CreatedDate
            });
        }

        public async Task<CaseDetailDto> GetCaseByCaseNumberAsync(string caseNumber)
        {
            var c = await _caseRepository.GetByCaseNumberAsync(caseNumber);
            if (c == null) return null;

            return new CaseDetailDto
            {
                CaseNumber = c.CaseNumber,
                ComplainantName = c.ComplainantName,
                DefendantName = c.DefendantName,
                AadhaarNumber = MaskingHelper.MaskAadhaar(c.AadhaarNumber ?? ""),
                PanNumber = MaskingHelper.MaskPan(c.PanNumber ?? ""),
                AccountNumber = MaskingHelper.MaskAccountNumber(c.AccountNumber ?? ""),
                BankName = c.BankName,
                OrderType = c.OrderType.ToString(),
                FreezeAmount = c.FreezeAmount,
                Status = c.Status.ToString(),
                CreatedDate = c.CreatedDate
            };
        }

        public void ValidateAttachments(Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan)
        {
            if (courtOrder == null || aadhaar == null || pan == null)
            {
                throw new ArgumentException("All three supporting documents are mandatory.");
            }

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var maxFileSize = 5 * 1024 * 1024; // 5 MB
            var files = new[] { courtOrder, aadhaar, pan };

            foreach (var doc in files)
            {
                if (doc.Length > maxFileSize)
                {
                    throw new ArgumentException("File size exceeds maximum allowed limit.");
                }

                var extension = Path.GetExtension(doc.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Unsupported file format.");
                }
            }
        }

        public Task<List<AttachmentDto>> ProcessAttachments(Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan)
        {
            // Stubbed implementation for file saving (since we don't save to DB or disk yet)
            var dtos = new List<AttachmentDto>
            {
                new AttachmentDto { FileName = courtOrder.FileName, FilePath = "/uploads/" + courtOrder.FileName, UploadedDate = DateTime.UtcNow },
                new AttachmentDto { FileName = aadhaar.FileName, FilePath = "/uploads/" + aadhaar.FileName, UploadedDate = DateTime.UtcNow },
                new AttachmentDto { FileName = pan.FileName, FilePath = "/uploads/" + pan.FileName, UploadedDate = DateTime.UtcNow }
            };
            
            return Task.FromResult(dtos);
        }

        public async Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto dto)
        {
            // Validate Documents
            ValidateAttachments(dto.CourtOrderFile, dto.AadhaarCopyFile, dto.PanCopyFile);

            // Validate Freeze Amount if FreezeAccount
            if (dto.OrderType == OrderType.FreezeAccount && (!dto.FreezeAmount.HasValue || dto.FreezeAmount.Value <= 0))
            {
                throw new ArgumentException("Freeze Amount is required for Freeze Account orders.");
            }

            // Process Attachments
            var attachments = await ProcessAttachments(dto.CourtOrderFile, dto.AadhaarCopyFile, dto.PanCopyFile);

            // Generate Case Number based on DB count to prevent duplicates across restarts
            var totalCases = await _caseRepository.GetTotalCountAsync();
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
            var caseNumber = $"CCMS-{dateStr}-{(totalCases + 1):D4}";
            var createdDate = DateTime.UtcNow;

            // Create Case Object
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

            await _caseRepository.AddAsync(newCase);

            // Return Response
            return new CaseResponseDto
            {
                CaseNumber = newCase.CaseNumber,
                Status = newCase.Status,
                CreatedDate = newCase.CreatedDate
            };
        }
    }
}
