using FluentValidation;
using SplitExpense.Models;
using SplitExpense.Models.DTO;

namespace SplitExpense.Validator
{
    public class AddFriendInGroupValidator : AbstractValidator<AddFriendInGroupRequest>
    {
        public AddFriendInGroupValidator()
        {
            RuleFor(x => x.GroupId)
            .GreaterThan(0).WithMessage("GroupId must be greater than zero.");

            RuleFor(x => x.FriendIds).Must(x => x.Count > 0).WithMessage("FriendIds must have at least one item.");
        }
    }
}
