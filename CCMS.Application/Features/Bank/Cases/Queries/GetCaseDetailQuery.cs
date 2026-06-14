using System;
using MediatR;
using CCMS.Application.Features.Bank.Cases.DTOs;

namespace CCMS.Application.Features.Bank.Cases.Queries
{
    public record GetCaseDetailQuery(Guid CaseId) : IRequest<CaseDetailDto>;
}
