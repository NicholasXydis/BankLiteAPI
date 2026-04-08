using BankLite.Application.DTOs;
using FluentValidation;

namespace BankLite.Application.Validators
{
    public class DepositWithdrawValidator : AbstractValidator<DepositWithdrawDto>
    {
        public DepositWithdrawValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}
