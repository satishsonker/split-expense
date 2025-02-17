using Microsoft.EntityFrameworkCore;
using SplitExpense.Data.Services;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory
{
    public class GroupFactory(SplitExpenseDbContext context, ISplitExpenseLogger logger, IUserContextService userContextService) : IGroupFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private int userId = userContextService.GetUserId();
        private readonly ISplitExpenseLogger _logger = logger;

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

        public async Task<Group> CreateAsync(Group request,List<int> members)
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

        public async Task<PagingResponse<Group>> GetAllAsync(PagingRequest request)
        {
            var query = _context.Groups
                .Include(x => x.Members)
                .ThenInclude(x => x.AddedUser)
                .Include(x => x.User)
                .Where(x => !x.IsDeleted && x.CreatedBy == userId)
                .AsQueryable();
            var response= new PagingResponse<Group>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
            response.Data.ForEach(x => x.Members.ForEach(y => y.Group = null));
            return response;
        }

        public async Task<Group?> GetAsync(int id)
        {
            return await _context.Groups
                 .Where(x => !x.IsDeleted && x.Id == id && x.CreatedBy == userId)
                 .FirstOrDefaultAsync();
        }

        public async Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request)
        {
            var query = _context.UserGroupMappings
                 .Include(x => x.Group)
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

        public async Task<int> UpdateAsync(Group request)
        {
            ArgumentNullException.ThrowIfNull(request);

            try
            {
                var oldData = await _context.Groups
                      .Where(x => !x.IsDeleted && x.Id == request.Id && x.CreatedBy == userId)
                      .FirstOrDefaultAsync() ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);

                oldData.Name = request.Name;
                _context.Groups.Update(oldData);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, "Group-UpdateAsync");
                throw new DbUpdateException(ex.Message);
            }
        }

        public async Task<UserGroupMapping> GetUserGroupMappingAsync(int id)
        {
            return await _context.UserGroupMappings
                .Include(x => x.Group)
                .Include(x => x.AddedUser)
                .Include(x => x.AddedByUser)
                .Where(x => !x.IsDeleted && x.Id == id)
               .FirstOrDefaultAsync();
        }
    }
}
