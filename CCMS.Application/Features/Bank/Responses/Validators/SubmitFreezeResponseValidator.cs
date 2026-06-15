using FluentValidation;
using CCMS.Application.Features.Bank.Responses.Commands;

namespace CCMS.Application.Features.Bank.Responses.Validators
{
    public class SubmitFreezeResponseValidator : AbstractValidator<SubmitFreezeResponseCommand>
    {
        public SubmitFreezeResponseValidator()
        {
            RuleFor(x => x.CaseId).NotEmpty();
            RuleFor(x => x.FreezeAmount).GreaterThan(0);
        }
    }
}
