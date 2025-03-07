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
        IConfiguration configuration) : IGroupFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly IFileUploadService _fileUploadService = fileUploadService;
        private int userId = userContextService.GetUserId();
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IConfiguration _configuration=configuration;

        public async Task<UserGroupMapping?> AddFriendInGroupAsync(AddFriendInGroupRequest request)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));
            var oldData = await _context.UserGroupMappings
                .Where(x => !x.IsDeleted && x.GroupId == request.GroupId && x.FriendId == request.FriendId)
                .FirstOrDefaultAsync();
            if(oldData!=null)
            {
                _logger.LogError(null, LogMessage.UserAlreadyAddedInGroup, "AddFriendInGroupAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordAlreadyExist);
            }

            try
            {
                var entity = await _context.UserGroupMappings.AddAsync(new()
                {
                    GroupId = request.GroupId,
                    FriendId = request.FriendId
                });

                return await _context.SaveChangesAsync() > 0 ? entity.Entity : null;
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

        public async Task<UserGroupMapping> GetUserGroupMappingAsync(int id)
        {
            return await _context.UserGroupMappings
                 .Include(x => x.Group)
                .ThenInclude(x => x.GroupDetail)
                .Include(x => x.Group)
                .ThenInclude(x => x.GroupType)
                .Include(x => x.AddedUser)
                .Include(x => x.AddedByUser)
                .Where(x => !x.IsDeleted && x.Id == id)
               .FirstOrDefaultAsync();
        }

        public async Task<List<ExpenseGroup>> GetRecentGroups(int userId)
        {
            int totalRecentGroups = _configuration.GetValue<int>("TotalRecentGroups");
            return await _context.Groups.Where(x => !x.IsDeleted && x.CreatedBy==userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(totalRecentGroups)
                .ToListAsync();
        }
    }
}
