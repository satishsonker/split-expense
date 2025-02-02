using Microsoft.EntityFrameworkCore;
using SplitExpense.Data.Services;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory
{
    public class FriendFactory(SplitExpenseDbContext context, IUserContextService userContext, ISplitExpenseLogger logger) : IFriendFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;
        private int userId = 0;// userContext.GetUserId(); ,

        public async Task<Friend> AddFriend(Friend request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var entity = await _context.Friends.AddAsync(request);
            if (await _context.SaveChangesAsync() > 0)
            {
                return entity.Entity;
            }
            _logger.LogError($"SaveChange return 0", null, "AddFriend(Friend request)");
            throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
        }

        public async Task<Friend> AddFriend(User request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var trans = await _context.Database.BeginTransactionAsync();
            Friend? friend = new()
            {
                UserId = userId,
            };

            if (request.UserId > 0)
            {
                friend.FriendId = request.UserId;
            }
            else
            {
                var oldData = _context.Users.Where(x => !x.IsDeleted && (x.Email == request.Email || x.Phone == request.Phone)).FirstOrDefault();
                if (oldData == null)
                {
                    var userEntity = await _context.Users.AddAsync(request);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        friend.FriendId = userEntity.Entity.UserId;
                    }
                    else
                    {
                        await trans.RollbackAsync();
                        _logger.LogError($"SaveChange return 0 while adding user", null, "AddFriend(User request)");
                       // throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
                    }
                }
                friend.FriendId = oldData.UserId;
            }

            var entity = await _context.Friends.AddAsync(friend);
            if (await _context.SaveChangesAsync() > 0)
            {
                await trans.CommitAsync();
                return entity.Entity;
            }
            await trans.RollbackAsync();
            _logger.LogError($"SaveChange return 0 while adding Friend", null, "AddFriend(User request)");
            //throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
            return default;
        }

        public async Task<bool> DeleteFriend(int friendId)
        {
            var oldData = await _context.Friends
                .Where(x => !x.IsDeleted && x.UserId == userId && x.FriendId == friendId)
                .FirstOrDefaultAsync();// ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);

            oldData.IsDeleted = true;
            _context.Friends.Update(oldData);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagingResponse<Friend>> GetAllFriend(PagingRequest request)
        {
            var query = _context.Friends
                .Include(x => x.User)
                .Include(x => x.UserFriend)
                .Where(x=>!x.IsDeleted && x.UserId==userId)
                .AsQueryable();

            return new PagingResponse<Friend>()
            {
                Data= await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                PageNo  = request.PageNo,
                PageSize = request.PageSize,
                RecordCounts = await query.CountAsync(),
            };
        }

        public async Task<PagingResponse<Friend>> SearchFriend(SearchRequest request)
        {
            var query = _context.Friends
                .Include(x => x.User)
                .Include(x => x.UserFriend)
                .Where(x => !x.IsDeleted && 
                            x.UserId == userId && 
                            (string.IsNullOrEmpty(request.SearchTerm) || 
                                x.UserFriend.FirstName.Contains(request.SearchTerm) || 
                                x.UserFriend.LastName.Contains(request.SearchTerm) ||
                                x.UserFriend.Phone.Contains(request.SearchTerm) ||
                                x.UserFriend.Email.Contains(request.SearchTerm)
                            )
                )
                .AsQueryable();

            return new PagingResponse<Friend>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                PageNo = request.PageNo,
                PageSize = request.PageSize,
                RecordCounts = await query.CountAsync(),
            };
        }
    }
}
