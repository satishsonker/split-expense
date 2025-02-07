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

            RuleFor(x => x.FriendId)
                .GreaterThan(0).WithMessage("FriendId must be greater than zero.");
        }
    }
}
