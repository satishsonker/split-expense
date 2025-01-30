using FluentValidation;
using SplitExpense.Models;
using SplitExpense.Models.DTO;

namespace SplitExpense.Validator
{
    public class AddFriendInGroupValidator : AbstractValidator<AddFriendInGroupRequest>
    {
        public AddFriendInGroupValidator()
        {
            RuleFor(x => x.FriendId)
               .LessThan(1).WithMessage("FriendId is required.")
               .NotNull().WithMessage("FriendId is required.");

            RuleFor(x => x.GroupId)
               .LessThan(1).WithMessage("GroupId is required.")
               .NotNull().WithMessage("GroupId is required.");
        }
    }
}
