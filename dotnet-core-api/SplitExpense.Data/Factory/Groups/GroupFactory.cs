using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SplitExpense.Data.Services;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.FileManagement.Service;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;
using System.Linq;

namespace SplitExpense.Data.Factory
{
    public class GroupFactory(SplitExpenseDbContext context, 
        ISplitExpenseLogger logger, 
        IUserContextService userContextService, 
        IFileUploadService fileUploadService,
        IConfiguration configuration, IUserContextService userContext) : IGroupFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly IFileUploadService _fileUploadService = fileUploadService;
        private int userId = userContextService.GetUserId();
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IConfiguration _configuration=configuration;
        private readonly IUserContextService _userContext=userContext;

        public async Task<List<UserGroupMapping?>> AddFriendInGroupAsync(AddFriendInGroupRequest request)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));
            var oldData = await _context.UserGroupMappings
                .Where(x => !x.IsDeleted && x.GroupId == request.GroupId && request.FriendIds.Contains(x.FriendId))
                .ToListAsync();
            if(oldData.Count==request.FriendIds.Count)
            {
                _logger.LogError(null, LogMessage.AllUserAlreadyAddedInGroup, "AddFriendInGroupAsync");
                throw new BusinessRuleViolationException(ErrorCodes.AllRecordAlreadyExist);
            }

            try
            {
                List<UserGroupMapping> mapList = [];
                request.FriendIds.ForEach(friendId =>
                {
                    if (friendId>0 && oldData.FirstOrDefault(x => x.FriendId == friendId) == null)
                    {
                        mapList.Add(new()
                        {
                            GroupId = request.GroupId,
                            FriendId = friendId
                        });
                    }
                });
                await _context.UserGroupMappings.AddRangeAsync(mapList);

                return await _context.SaveChangesAsync() > 0 ? mapList : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "AddFriendInGroup");
                throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
            }
        }

        public async Task<ExpenseGroup> CreateAsync(ExpenseGroup request,List<int> members)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                var trans = await _context.Database.BeginTransactionAsync();
                request.UserId=_userContext.GetUserId();

                var entity = await _context.Groups.AddAsync(request);
                if (await _context.SaveChangesAsync() > 0)
                {
                    var userGroupMap = new List<UserGroupMapping>()
                    {
                        new(){FriendId=userId,GroupId=entity.Entity.Id}
                    };
                    if(members != null && members.Count>0)
                    {
                        members.ForEach(memberId => userGroupMap.Add(new() { FriendId = memberId, GroupId = entity.Entity.Id }));
                        
                    }
                    _context.UserGroupMappings.AddRange(userGroupMap);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        await trans.CommitAsync();
                        return entity.Entity;
                    }
                    throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
                }
                throw new DbUpdateException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "Group-CreateAsync");
                throw new DbUpdateException(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var oldData = await _context.Groups
                        .Where(x => !x.IsDeleted && x.Id == id && x.CreatedBy == userId)
                        .FirstOrDefaultAsync() ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);

                oldData.IsDeleted = true;
                _context.Groups.Update(oldData);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "Group-DeleteAsync");
                throw new DbUpdateException(ex.Message);
            }

        }

        public async Task<PagingResponse<ExpenseGroup>> GetAllAsync(PagingRequest request)
        {
            var query = _context.Groups
                .Include(x=>x.GroupDetail)
                .Include(x => x.GroupType)
                .Include(x => x.Members)
                .ThenInclude(x => x.AddedUser)
                .Include(x => x.User)
                .Where(x => !x.IsDeleted && x.CreatedBy == userId)
                .AsQueryable();
            var response= new PagingResponse<ExpenseGroup>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
            response.Data.ForEach(x => x.Members.ForEach(y => y.Group = null));
            return response;
        }

        public async Task<ExpenseGroup?> GetAsync(int id)
        {
            return await _context.Groups
                .Include(x => x.GroupDetail)
                .Include(x => x.GroupType)
                .Include(x=>x.Members)
                .ThenInclude(x=>x.AddedUser)
                 .Include(x => x.Members)
                .ThenInclude(x => x.AddedByUser)
                 .Where(x => !x.IsDeleted && x.Id == id && x.CreatedBy == userId)
                 .FirstOrDefaultAsync();
        }

        public async Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request)
        {
            var query = _context.UserGroupMappings
                .Include(x => x.Group)
                .ThenInclude(x => x.GroupDetail)
                .Include(x => x.Group)
                .ThenInclude(x => x.GroupType)
                .Include(x => x.AddedUser)
                .Include(x => x.AddedByUser)
                .Where(x => !x.IsDeleted && x.CreatedBy == userId && (string.IsNullOrEmpty(request.SearchTerm) || x.Group.Name.Contains(request.SearchTerm)))
                .AsQueryable();

            return new PagingResponse<UserGroupMapping>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
        }

        public async Task<int> UpdateAsync(ExpenseGroup request,List<int> members)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            string oldGroupImage, oldThumbImage;
            try
            {
                var existingGroup = await _context.Groups
                    .Include(x => x.Members)
                    .Include(x => x.GroupDetail)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted) ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);

                oldGroupImage = existingGroup.ImagePath;
                oldThumbImage = existingGroup.ThumbImagePath;
                // Update basic group properties
                existingGroup.Name = request.Name;
                existingGroup.Icon = request.Icon;
                existingGroup.GroupTypeId = request.GroupTypeId;

                if (!string.IsNullOrEmpty(request.ImagePath))
                {
                    existingGroup.ImagePath = request.ImagePath;
                    existingGroup.ThumbImagePath = request.ThumbImagePath;
                }

                // Update group details
                if (request.GroupDetail != null)
                {
                    if (existingGroup.GroupDetail == null)
                    {
                        request.GroupDetail.GroupId = request.Id;
                        await _context.GroupDetails.AddAsync(request.GroupDetail);
                    }
                    else
                    {
                        existingGroup.GroupDetail.EnableBalanceAlert = request.GroupDetail.EnableBalanceAlert;
                        existingGroup.GroupDetail.EnableGroupDate = request.GroupDetail.EnableGroupDate;
                        existingGroup.GroupDetail.EnableSettleUpReminders = request.GroupDetail.EnableSettleUpReminders;
                        existingGroup.GroupDetail.MaxGroupBudget = request.GroupDetail.MaxGroupBudget;
                        existingGroup.GroupDetail.StartDate = request.GroupDetail.StartDate;
                        existingGroup.GroupDetail.EndDate = request.GroupDetail.EndDate;
                    }
                }

                // Update user mappings
                if (members != null && members.Count>0)
                {
                    // Get existing member IDs
                    var existingMemberIds = existingGroup.Members?.Select(m => m.FriendId).ToList() ?? [];

                    // Find members to add and remove
                    var membersToAdd = members.Except(existingMemberIds).ToList();
                    var membersToRemove = existingMemberIds.Except(members.Select(x=>x).ToList()).ToList();

                    // Remove old mappings
                    if (membersToRemove.Count != 0)
                    {
                        var mappingsToRemove = await _context.UserGroupMappings
                            .Where(m => m.GroupId == request.Id && membersToRemove.Contains(m.FriendId))
                            .ToListAsync();
                        _context.UserGroupMappings.RemoveRange(mappingsToRemove);
                    }

                    // Add new mappings
                    if (membersToAdd.Count != 0)
                    {
                        // Verify all users exist
                        var existingUsers = await _context.Users
                            .Where(u => membersToAdd.Contains(u.UserId))
                            .Select(u => u.UserId)
                            .ToListAsync();

                        var invalidUserIds = membersToAdd.Except(existingUsers).ToList();
                        if (invalidUserIds.Count != 0)
                        {
                            throw new BusinessRuleViolationException(
                                ErrorCodes.InvalidData.GetDescription(),
                                $"Invalid user IDs: {string.Join(", ", invalidUserIds)}");
                        }

                        var newMappings = membersToAdd.Select(friendId => new UserGroupMapping
                        {
                            GroupId = request.Id,
                            FriendId = friendId
                        });

                        await _context.UserGroupMappings.AddRangeAsync(newMappings);
                    }
                }

                var result = await _context.SaveChangesAsync();
                if (oldGroupImage != null && string.IsNullOrEmpty(request.ImagePath))
                {
                    await _fileUploadService.DeleteFileAsync(oldGroupImage, oldThumbImage);
                }
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await _fileUploadService.DeleteFileAsync(request.ImagePath, request.ThumbImagePath);
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error updating group with id {request.Id}");
                throw;
            }
        }

        public async Task<List<UserGroupMapping>> GetUserGroupMappingAsync(List<int> ids)
        {
            return await _context.UserGroupMappings
                 .Include(x => x.Group)
                .ThenInclude(x => x.GroupDetail)
                .Include(x => x.Group)
                .ThenInclude(x => x.GroupType)
                .Include(x => x.AddedUser)
                .Include(x => x.AddedByUser)
                .Where(x => !x.IsDeleted && ids.Contains(x.Id))
               .ToListAsync();
        }

        public async Task<List<ExpenseGroup>> GetRecentGroups()
        {
            int totalRecentGroups = _configuration.GetValue<int>("Group:TotalRecentGroups");
            return await _context.Groups.Where(x => !x.IsDeleted && x.CreatedBy==userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(totalRecentGroups)
                .ToListAsync();
        }

        public async Task<bool> RemoveFriendInGroupAsync(RemoveFriendFromGroup request)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));
            var oldData = await _context.UserGroupMappings
                .Where(x => !x.IsDeleted && x.GroupId == request.GroupId && x.FriendId==request.FriendId)
                .FirstOrDefaultAsync();
            if (oldData==null)
            {
                _logger.LogError(null, LogMessage.RecordNotExist, "RemoveFriendInGroupAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            }

            try
            {

                oldData.IsDeleted = true;
                _context.UserGroupMappings.Update(oldData);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "RemoveFriendInGroupAsync");
                throw new BusinessRuleViolationException(ErrorCodes.UnableToUpdateRecord);
            }
        }

        public async Task<GroupSummaryResponse> GetGroupSummaryAsync(int groupId, int userId)
        {
            try
            {
                // Get all expenses for this group
                var expenses = await _context.Expenses
                    .Where(x => !x.IsDeleted && x.GroupId == groupId)
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .ToListAsync();

                // Get all transactions for this group (if transactions are group-specific)
                // For now, we'll calculate based on expenses only

                decimal totalExpenses = expenses.Sum(e => e.Amount);
                decimal youOwe = 0;
                decimal youAreOwed = 0;

                // Get all member user IDs in this group
                var memberUserIds = await _context.UserGroupMappings
                    .Where(m => !m.IsDeleted && m.GroupId == groupId)
                    .Select(m => m.FriendId)
                    .ToListAsync();

                // Get user details for members
                var members = await _context.Users
                    .Where(u => memberUserIds.Contains(u.UserId) && !u.IsDeleted)
                    .ToListAsync();

                var memberBalances = new Dictionary<int, GroupMemberBalanceResponse>();

                // Initialize member balances
                foreach (var member in members)
                {
                    memberBalances[member.UserId] = new GroupMemberBalanceResponse
                    {
                        UserId = member.UserId,
                        FirstName = member.FirstName ?? "",
                        LastName = member.LastName ?? "",
                        Email = member.Email ?? "",
                        ProfilePicture = member.ThumbProfilePicture,
                        Balance = 0
                    };
                }

                // Calculate balances from expenses
                foreach (var expense in expenses)
                {
                    var paidByMe = expense.PaidByUserId == userId;

                    if (paidByMe)
                    {
                        // I paid - others owe me
                        if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                        {
                            foreach (var share in expense.ExpenseShares.Where(s => s.UserId != userId))
                            {
                                youAreOwed += share.AmountOwed;
                                if (memberBalances.ContainsKey(share.UserId))
                                {
                                    memberBalances[share.UserId].Balance -= share.AmountOwed; // Negative = they owe
                                }
                            }
                        }
                    }
                    else
                    {
                        // Someone else paid - check if I owe
                        if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                        {
                            var myShare = expense.ExpenseShares.FirstOrDefault(s => s.UserId == userId);
                            if (myShare != null)
                            {
                                youOwe += myShare.AmountOwed;
                                if (memberBalances.ContainsKey(expense.PaidByUserId))
                                {
                                    memberBalances[expense.PaidByUserId].Balance += myShare.AmountOwed; // Positive = they're owed
                                }
                            }
                        }
                    }
                }

                // Get transactions for this group (if we track group transactions)
                // For now, we'll use all transactions between group members
                var groupTransactions = await _context.Transactions
                    .Where(t => !t.IsDeleted &&
                        memberUserIds.Contains(t.FromUserId) &&
                        memberUserIds.Contains(t.ToUserId))
                    .ToListAsync();

                // Adjust balances based on transactions
                foreach (var trans in groupTransactions)
                {
                    if (trans.FromUserId == userId)
                    {
                        // I paid to someone in the group
                        if (memberBalances.ContainsKey(trans.ToUserId))
                        {
                            if (memberBalances[trans.ToUserId].Balance < 0)
                            {
                                // They owe me - reduce what they owe
                                var reduction = Math.Min(trans.Amount, Math.Abs(memberBalances[trans.ToUserId].Balance));
                                memberBalances[trans.ToUserId].Balance += reduction;
                                youAreOwed -= reduction;
                            }
                            else
                            {
                                // They don't owe me - increase what I owe them
                                memberBalances[trans.ToUserId].Balance += trans.Amount;
                                youOwe += trans.Amount;
                            }
                        }
                    }
                    else if (trans.ToUserId == userId)
                    {
                        // Someone in the group paid to me
                        if (memberBalances.ContainsKey(trans.FromUserId))
                        {
                            if (memberBalances[trans.FromUserId].Balance > 0)
                            {
                                // I owe them - reduce what I owe
                                var reduction = Math.Min(trans.Amount, memberBalances[trans.FromUserId].Balance);
                                memberBalances[trans.FromUserId].Balance -= reduction;
                                youOwe -= reduction;
                            }
                            else
                            {
                                // I don't owe them - increase what they owe me
                                memberBalances[trans.FromUserId].Balance -= trans.Amount;
                                youAreOwed += trans.Amount;
                            }
                        }
                    }
                }

                var yourBalance = youAreOwed - youOwe;

                return new GroupSummaryResponse
                {
                    GroupId = groupId,
                    TotalExpenses = totalExpenses,
                    YouOwe = youOwe,
                    YouAreOwed = youAreOwed,
                    YourBalance = yourBalance,
                    MemberBalances = memberBalances.Values.OrderByDescending(m => m.Balance).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "GetGroupSummaryAsync");
                throw new BusinessRuleViolationException(ErrorCodes.GeneralError);
            }
        }

        public async Task<Dictionary<int, GroupSummaryResponse>> GetGroupsSummaryAsync(List<int> groupIds, int userId)
        {
            try
            {
                var summaries = new Dictionary<int, GroupSummaryResponse>();

                foreach (var groupId in groupIds)
                {
                    var summary = await GetGroupSummaryAsync(groupId, userId);
                    summaries[groupId] = summary;
                }

                return summaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "GetGroupsSummaryAsync");
                throw new BusinessRuleViolationException(ErrorCodes.GeneralError);
            }
        }

        public async Task<GroupExpenseBreakdownResponse> GetGroupExpenseBreakdownAsync(int groupId, int userId)
        {
            try
            {
                // Get all expenses for this group
                var expenses = await _context.Expenses
                    .Where(x => !x.IsDeleted && x.GroupId == groupId)
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .ToListAsync();

                // Get all member user IDs in this group
                var memberUserIds = await _context.UserGroupMappings
                    .Where(m => !m.IsDeleted && m.GroupId == groupId)
                    .Select(m => m.FriendId)
                    .ToListAsync();

                // Get user details for members
                var members = await _context.Users
                    .Where(u => memberUserIds.Contains(u.UserId) && !u.IsDeleted)
                    .ToListAsync();

                var memberBreakdown = new Dictionary<int, MemberExpenseBreakdown>();

                // Initialize member breakdown
                foreach (var member in members)
                {
                    memberBreakdown[member.UserId] = new MemberExpenseBreakdown
                    {
                        UserId = member.UserId,
                        FirstName = member.FirstName ?? "",
                        LastName = member.LastName ?? "",
                        Email = member.Email ?? "",
                        ProfilePicture = member.ThumbProfilePicture,
                        TotalPaid = 0,
                        TotalOwed = 0,
                        NetAmount = 0,
                        Percentage = 0
                    };
                }

                decimal totalSpending = 0;

                // Calculate breakdown from expenses
                foreach (var expense in expenses)
                {
                    totalSpending += expense.Amount;

                    // Track who paid
                    if (memberBreakdown.ContainsKey(expense.PaidByUserId))
                    {
                        memberBreakdown[expense.PaidByUserId].TotalPaid += expense.Amount;
                    }

                    // Track who owes
                    if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                    {
                        foreach (var share in expense.ExpenseShares)
                        {
                            if (memberBreakdown.ContainsKey(share.UserId))
                            {
                                memberBreakdown[share.UserId].TotalOwed += share.AmountOwed;
                            }
                        }
                    }
                }

                // Calculate net amounts and percentages
                foreach (var member in memberBreakdown.Values)
                {
                    member.NetAmount = member.TotalPaid - member.TotalOwed;
                    if (totalSpending > 0)
                    {
                        member.Percentage = (member.TotalPaid / totalSpending) * 100;
                    }
                }

                return new GroupExpenseBreakdownResponse
                {
                    GroupId = groupId,
                    TotalSpending = totalSpending,
                    MemberBreakdown = memberBreakdown.Values
                        .OrderByDescending(m => m.TotalPaid)
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "GetGroupExpenseBreakdownAsync");
                throw new BusinessRuleViolationException(ErrorCodes.GeneralError);
            }
        }
    }
}
