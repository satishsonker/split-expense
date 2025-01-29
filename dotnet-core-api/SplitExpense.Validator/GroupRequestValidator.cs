using FluentValidation;
using SplitExpense.Models;

namespace SplitExpense.Validator
{
    public class GroupRequestValidator : AbstractValidator<GroupRequest>
    {
        public GroupRequestValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Group name is required.")
               .NotNull().WithMessage("Group name is required.")
               .MaximumLength(200).WithMessage("Group name must not exceed 200 characters.");
        }
    }
}
