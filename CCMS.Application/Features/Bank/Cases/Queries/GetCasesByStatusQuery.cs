using System.Collections.Generic;
using MediatR;
using CCMS.Application.Features.Bank.Cases.DTOs;

namespace CCMS.Application.Features.Bank.Cases.Queries
{
    public record GetCasesByStatusQuery(string StatusCategory) : IRequest<IEnumerable<CaseListDto>>;
}
