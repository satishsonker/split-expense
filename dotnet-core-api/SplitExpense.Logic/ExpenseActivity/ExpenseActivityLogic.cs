using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Logic
{
    public class ExpenseActivityLogic : IExpenseActivityLogic
    {
        private readonly IExpenseActivityFactory _expenseActivityFactory;
        private readonly IMapper _mapper;
        private readonly ISplitExpenseLogger _logger;

        public ExpenseActivityLogic(
            IExpenseActivityFactory expenseActivityFactory,
            IMapper mapper,
            ISplitExpenseLogger logger)
        {
            _expenseActivityFactory = expenseActivityFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ExpenseActivityResponse> CreateAsync(ExpenseActivityRequest request)
        {
            try
            {
                var activity = _mapper.Map<ExpenseActivity>(request);

                var result = await _expenseActivityFactory.CreateAsync(activity);
                return _mapper.Map<ExpenseActivityResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense activity");
                throw;
            }
        }

        public async Task<bool> CreateRangeAsync(List<ExpenseActivityRequest> request)
        {
            try
            {
                var activity = _mapper.Map<List<ExpenseActivity>>(request);

               return await _expenseActivityFactory.CreateRangeAsync(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense activity");
                throw;
            }
        }

        public string GetActivityMessage(ExpenseActivityTypeEnum activityType, Dictionary<string, string> data)
        {
            var message = activityType switch
            {
                ExpenseActivityTypeEnum.GroupCreated => "{{creator}} created a group {{groupName}}",
                ExpenseActivityTypeEnum.MemberAddedInGroup => "{{adder}} added {{addedUser}} to group {{groupName}}", 
                ExpenseActivityTypeEnum.MemberAddedByGroupMember => "{{addedUser}} added by {{addedBy}} to group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseAdded => "{{creator}} added expense {{expenseName}} of {{amount}} in group {{groupName}}",
                ExpenseActivityTypeEnum.MemberDeletedInGroup => "{{remover}} removed {{removedUser}} from group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseDeleted => "{{deleter}} deleted expense {{expenseName}} from group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseUpdated => "{{updater}} updated expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.GroupDeleted => "{{deleter}} deleted group {{groupName}}",
                ExpenseActivityTypeEnum.MemberUpdatedInGroup => "{{updater}} updated member {{memberName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.MemberLeftGroup => "{{member}} left group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseSettled => "{{settler}} settled expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseUnsettled => "{{user}} unsettled expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseReopened => "Expense {{expenseName}} reopened in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseClosed => "Expense {{expenseName}} closed in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseReopenedByAdmin => "Admin reopened expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseClosedByAdmin => "Admin closed expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseSettledByAdmin => "Admin settled expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseUnsettledByAdmin => "Admin unsettled expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.MemberAddedInGroupByAdmin => "Admin added {{addedUser}} to group {{groupName}}",
                ExpenseActivityTypeEnum.MemberDeletedInGroupByAdmin => "Admin removed {{removedUser}} from group {{groupName}}",
                ExpenseActivityTypeEnum.MemberUpdatedInGroupByAdmin => "Admin updated member {{memberName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.MemberLeftGroupByAdmin => "Admin removed {{member}} from group {{groupName}}",
                ExpenseActivityTypeEnum.GroupDeletedByAdmin => "Admin deleted group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseAddedByAdmin => "Admin added expense {{expenseName}} of {{amount}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseDeletedByAdmin => "Admin deleted expense {{expenseName}} from group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseUpdatedByAdmin => "Admin updated expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.GroupCreatedByAdmin => "Admin created group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseReopenedByMember => "Member reopened expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseClosedByMember => "Member closed expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseSettledByMember => "Member settled expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseUnsettledByMember => "Member unsettled expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.MemberAddedInGroupByMember => "Member added {{addedUser}} to group {{groupName}}",
                ExpenseActivityTypeEnum.MemberDeletedInGroupByMember => "Member removed {{removedUser}} from group {{groupName}}",
                ExpenseActivityTypeEnum.MemberUpdatedInGroupByMember => "Member updated {{memberName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.MemberLeftGroupByMember => "Member left group {{groupName}}",
                ExpenseActivityTypeEnum.GroupDeletedByMember => "Member deleted group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseAddedByMember => "Member added expense {{expenseName}} of {{amount}} in group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseDeletedByMember => "Member deleted expense {{expenseName}} from group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseUpdatedByMember => "Member updated expense {{expenseName}} in group {{groupName}}",
                ExpenseActivityTypeEnum.GroupCreatedByMember => "Member created group {{groupName}}",
                ExpenseActivityTypeEnum.ExpenseReopenedByGroup => "Group reopened expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseClosedByGroup => "Group closed expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseSettledByGroup => "Group settled expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseUnsettledByGroup => "Group unsettled expense {{expenseName}}",
                ExpenseActivityTypeEnum.MemberAddedInGroupByGroup => "Group added {{addedUser}}",
                ExpenseActivityTypeEnum.MemberDeletedInGroupByGroup => "Group removed {{removedUser}}",
                ExpenseActivityTypeEnum.MemberUpdatedInGroupByGroup => "Group updated member {{memberName}}",
                ExpenseActivityTypeEnum.MemberLeftGroupByGroup => "Member left group {{groupName}}",
                ExpenseActivityTypeEnum.GroupDeletedByGroup => "Group {{groupName}} deleted",
                ExpenseActivityTypeEnum.ExpenseAddedByGroup => "Expense {{expenseName}} of {{amount}} added to group",
                ExpenseActivityTypeEnum.ExpenseDeletedByGroup => "Expense {{expenseName}} deleted from group",
                ExpenseActivityTypeEnum.ExpenseUpdatedByGroup => "Expense {{expenseName}} updated in group",
                ExpenseActivityTypeEnum.GroupCreatedByGroup => "Group {{groupName}} created",
                ExpenseActivityTypeEnum.ExpenseReopenedByGroupAdmin => "Group admin reopened expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseClosedByGroupAdmin => "Group admin closed expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseSettledByGroupAdmin => "Group admin settled expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseUnsettledByGroupAdmin => "Group admin unsettled expense {{expenseName}}",
                ExpenseActivityTypeEnum.MemberAddedInGroupByGroupAdmin => "Group admin added {{addedUser}}",
                ExpenseActivityTypeEnum.MemberDeletedInGroupByGroupAdmin => "Group admin removed {{removedUser}}",
                ExpenseActivityTypeEnum.MemberUpdatedInGroupByGroupAdmin => "Group admin updated member {{memberName}}",
                ExpenseActivityTypeEnum.MemberLeftGroupByGroupAdmin => "Member left group by group admin",
                ExpenseActivityTypeEnum.MemberDeletedByGroupAdmin => "Member deleted by group admin",
                ExpenseActivityTypeEnum.MemberAddedByGroupAdmin => "Member added by group admin",
                ExpenseActivityTypeEnum.MemberUpdatedByGroupAdmin => "Member updated by group admin",
                ExpenseActivityTypeEnum.MemberLeftByGroupAdmin => "Member left by group admin action",
                ExpenseActivityTypeEnum.GroupDeletedByGroupAdmin => "Group deleted by group admin",
                ExpenseActivityTypeEnum.ExpenseAddedByGroupAdmin => "Group admin added expense {{expenseName}} of {{amount}}",
                ExpenseActivityTypeEnum.ExpenseDeletedByGroupAdmin => "Group admin deleted expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseUpdatedByGroupAdmin => "Group admin updated expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseReopenedByMemberAdmin => "Member admin reopened expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseClosedByMemberAdmin => "Member admin closed expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseSettledByMemberAdmin => "Member admin settled expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseUnsettledByMemberAdmin => "Member admin unsettled expense {{expenseName}}",
                ExpenseActivityTypeEnum.MemberAddedInGroupByMemberAdmin => "Member admin added {{addedUser}}",
                ExpenseActivityTypeEnum.MemberDeletedInGroupByMemberAdmin => "Member admin removed {{removedUser}}",
                ExpenseActivityTypeEnum.MemberUpdatedInGroupByMemberAdmin => "Member admin updated {{memberName}}",
                ExpenseActivityTypeEnum.MemberLeftByMemberAdmin => "Member left by member admin action",
                ExpenseActivityTypeEnum.MemberDeletedByMemberAdmin => "Member deleted by member admin",
                ExpenseActivityTypeEnum.MemberAddedByMemberAdmin => "Member added by member admin",
                ExpenseActivityTypeEnum.MemberUpdatedByMemberAdmin => "Member updated by member admin",
                ExpenseActivityTypeEnum.GroupDeletedByMemberAdmin => "Group deleted by member admin",
                ExpenseActivityTypeEnum.ExpenseAddedByMemberAdmin => "Member admin added expense {{expenseName}} of {{amount}}",
                ExpenseActivityTypeEnum.ExpenseDeletedByMemberAdmin => "Member admin deleted expense {{expenseName}}",
                ExpenseActivityTypeEnum.ExpenseUpdatedByMemberAdmin => "Member admin updated expense {{expenseName}}",
                _ => string.Empty,
            };

            foreach (var item in data)
            {
                message = message.Replace("{{" + item.Key + "}}", item.Value);
            }
            return message;
        }

        public async Task<PagingResponse<ExpenseActivityResponse>> GetAllAsync(PagingRequest request)
        {
            try
            {
                var result = await _expenseActivityFactory.GetAllAsync(request);
                return _mapper.Map<PagingResponse<ExpenseActivityResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all expense activities");
                throw;
            }
        }

        public async Task<PagingResponse<ExpenseActivityResponse>> SearchAsync(SearchRequest request)
        {
            try
            {
                var result = await _expenseActivityFactory.SearchAsync(request);
                return _mapper.Map<PagingResponse<ExpenseActivityResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching expense activities");
                throw;
            }
        }
    }
}