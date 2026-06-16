using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Application.DTOs.Court;
using CCMS.Application.Interfaces;
using CCMS.Application.Interfaces.Repositories;
using CCMS.Application.Interfaces.Services;
using CCMS.Domain.Entities;
using CCMS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CCMS.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMaskingService _maskingService;

        public CourtService(ICaseRepository caseRepository, IFileStorageService fileStorageService, IMaskingService maskingService)
        {
            _caseRepository = caseRepository;
            _fileStorageService = fileStorageService;
            _maskingService = maskingService;
        }

        public async Task<CourtDashboardDto> GetDashboardAsync()
        {
            var allCases = await _caseRepository.GetAllAsync();
            return new CourtDashboardDto
            {
                TotalCases = allCases.Count(),
                PendingCases = allCases.Count(c => c.Status == CaseStatus.Pending),
                AccountValidatedCases = allCases.Count(c => c.Status == CaseStatus.AccountValidated),
                AccountNotFoundCases = allCases.Count(c => c.Status == CaseStatus.AccountNotFound),
                FreezeAppliedCases = allCases.Count(c => c.Status == CaseStatus.FreezeApplied),
                BalanceProvidedCases = allCases.Count(c => c.Status == CaseStatus.BalanceProvided)
            };
        }

        public async Task<IEnumerable<CaseListDto>> GetCasesAsync()
        {
            var allCases = await _caseRepository.GetAllAsync();
            return allCases.Select(c => new CaseListDto
            {
                CaseNumber = c.CaseNumber,
                DefendantName = c.DefendantName,
                OrderType = c.OrderType.ToString(),
                Status = c.Status.ToString(),
                CreatedDate = c.CreatedDate
            });
        }

        public async Task<CaseDetailDto> GetCaseByNumberAsync(string caseNumber)
        {
            var caseEntity = await _caseRepository.GetByCaseNumberAsync(caseNumber);
            if (caseEntity == null)
            {
                return null;
            }

            var maskedAadhaar = _maskingService.MaskAadhaar(caseEntity.AadhaarNumber);
            var maskedPan = _maskingService.MaskPAN(caseEntity.PanNumber);
            var maskedAccount = _maskingService.MaskAccountNumber(caseEntity.AccountNumber);

            return new CaseDetailDto
            {
                CaseNumber = caseEntity.CaseNumber,
                DefendantName = caseEntity.DefendantName,
                ComplainantName = caseEntity.ComplainantName,
                AadhaarNumber = maskedAadhaar,
                PanNumber = maskedPan,
                AccountNumber = maskedAccount,
                BankName = caseEntity.BankName,
                OrderType = caseEntity.OrderType.ToString(),
                FreezeAmount = caseEntity.FreezeAmount,
                Status = caseEntity.Status.ToString(),
                CreatedDate = caseEntity.CreatedAt,
                Attachments = caseEntity.Attachments?.Select(a => new AttachmentDto
                {
                    FileName = a.FileName,
                    FilePath = $"/api/attachments/{System.IO.Path.GetFileName(a.StoragePath)}",
                    UploadedDate = a.CreatedAt
                }).ToList(),
                BankResponse = (caseEntity.Status == CaseStatus.FreezeApplied || 
                                caseEntity.Status == CaseStatus.BalanceProvided || 
                                caseEntity.Status == CaseStatus.AccountNotFound) ? new
                {
                    accountNumber = string.IsNullOrEmpty(caseEntity.MatchedAccountNumber) ? null : _maskingService.MaskAccountNumber(caseEntity.MatchedAccountNumber),
                    balance = caseEntity.CaseResponse?.BalanceAmount,
                    accountStatus = caseEntity.MatchedAccountStatus,
                    remarks = caseEntity.CaseResponse?.Remarks ?? (caseEntity.Status == CaseStatus.AccountNotFound ? "No matching account found in bank records." : null),
                    respondedAt = caseEntity.CaseResponse?.RespondedAt ?? caseEntity.UpdatedAt,
                    responseType = caseEntity.CaseResponse != null ? (int)caseEntity.CaseResponse.ResponseType : -1,
                    freezeAmount = caseEntity.CaseResponse?.FreezeAmount
                } : null
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

        public async Task<List<Attachment>> ProcessAttachments(Guid caseId, Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan)
        {
            var attachments = new List<Attachment>();
            
            var courtOrderPath = await _fileStorageService.SaveFileAsync(courtOrder);
            attachments.Add(new Attachment { Id = Guid.NewGuid(), CaseId = caseId, FileName = courtOrder.FileName, StoragePath = courtOrderPath, CreatedAt = DateTime.UtcNow });

            var aadhaarPath = await _fileStorageService.SaveFileAsync(aadhaar);
            attachments.Add(new Attachment { Id = Guid.NewGuid(), CaseId = caseId, FileName = aadhaar.FileName, StoragePath = aadhaarPath, CreatedAt = DateTime.UtcNow });

            var panPath = await _fileStorageService.SaveFileAsync(pan);
            attachments.Add(new Attachment { Id = Guid.NewGuid(), CaseId = caseId, FileName = pan.FileName, StoragePath = panPath, CreatedAt = DateTime.UtcNow });
            
            return attachments;
        }

        public async Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto dto)
        {
            // Validate Documents
            ValidateAttachments(dto.CourtOrderFile, dto.AadhaarCopyFile, dto.PanCopyFile);

            // Validate Freeze Amount if Freeze
            if (dto.OrderType == OrderType.Freeze && (!dto.FreezeAmount.HasValue || dto.FreezeAmount.Value <= 0))
            {
                throw new ArgumentException("Freeze Amount is required for Freeze orders.");
            }

            var caseId = Guid.NewGuid();
            var attachments = await ProcessAttachments(caseId, dto.CourtOrderFile, dto.AadhaarCopyFile, dto.PanCopyFile);

            var today = DateTime.UtcNow;
            var dailyCount = await _caseRepository.GetDailyCaseCountAsync(today);
            var sequenceNumber = (dailyCount + 1).ToString("D4");
            var caseNumber = $"CCMS-{today:yyyyMMdd}-{sequenceNumber}";

            var newCase = new Case
            {
                Id = caseId,
                CaseNumber = caseNumber,
                ComplainantName = dto.ComplainantName,
                DefendantName = dto.DefendantName,
                AadhaarNumber = dto.AadhaarNumber,
                PanNumber = dto.PanNumber,
                AccountNumber = dto.AccountNumber,
                BankName = dto.BankName,
                OrderType = dto.OrderType,
                FreezeAmount = dto.FreezeAmount ?? 0m,
                Status = CaseStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                Attachments = attachments
            };

            await _caseRepository.AddAsync(newCase);

            return new CaseResponseDto
            {
                Id = newCase.Id,
                CaseNumber = newCase.CaseNumber,
                Status = newCase.Status,
                CreatedDate = newCase.CreatedDate
            };
        }
    }
}
