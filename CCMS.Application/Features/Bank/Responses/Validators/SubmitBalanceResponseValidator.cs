using FluentValidation;
using CCMS.Application.Features.Bank.Responses.Commands;

namespace CCMS.Application.Features.Bank.Responses.Validators
{
    public class SubmitBalanceResponseValidator : AbstractValidator<SubmitBalanceResponseCommand>
    {
        public SubmitBalanceResponseValidator()
        {
            RuleFor(x => x.CaseId).NotEmpty();
            RuleFor(x => x.BalanceAmount).GreaterThanOrEqualTo(0);
        }
    }
}
