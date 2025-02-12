using Microsoft.EntityFrameworkCore;
using SplitExpense.Models;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Models.Common;
using SplitExpense.Logger;
using SplitExpense.SharedResource;
using Azure.Core;
using SplitExpense.Data.Services;
using SplitExpense.Models.DbModels;

namespace SplitExpense.Data.Factory
{
    public class ContactFactory(SplitExpenseDbContext context, ISplitExpenseLogger logger, IUserContextService userContext) : IContactFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;
        private int userId = userContext.GetUserId();

        public async Task<bool> AddInContactListAsync(int contactUserId)
        {
           var alreadyInContact = _context.Contacts
                .Where(x => !x.IsDeleted && x.UserId == userId && x.ContactId == contactUserId)
                .FirstOrDefault();
            if (alreadyInContact != null)
            {
                _logger.LogError(LogMessage.UserAlreadyAddedInContact.ToFormattedString(), ErrorCodes.RecordAlreadyExist.GetDescription(), "ContactFactory-AddInContactListAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordAlreadyExist);
            }
            var contact = new Contact()
            {
                UserId = userId,
                ContactId = contactUserId
            };
            var entity = _context.Contacts.Add(contact);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Contact> CreateAsync(User request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var trans = await _context.Database.BeginTransactionAsync();
            Contact? friend = new()
            {
                UserId = userId,
                ContactId=0
            };

            if (request.UserId > 0)
            {
                friend.ContactId = request.UserId;
            }
            else
            {
                var oldData = _context.Users.Where(x => !x.IsDeleted && (x.Email == request.Email || x.Phone == request.Phone)).FirstOrDefault();
                if (oldData == null)
                {
                    var userEntity = await _context.Users.AddAsync(request);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        friend.ContactId = userEntity.Entity.UserId;
                    }
                    else
                    {
                        await trans.RollbackAsync();
                        _logger.LogError($"SaveChange return 0 while adding user", null, "AddFriend(User request)");
                        throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
                    }
                }
                else
                friend.ContactId = oldData.UserId;
            }

            var entity = await _context.Contacts.AddAsync(friend);
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

        public async Task<bool> DeleteAsync(int id)
        {
            var oldData = await _context.Contacts
            .Where(x => !x.IsDeleted && x.ContactId==id && x.CreatedBy==userId)
                .FirstOrDefaultAsync();
            if (oldData == null)
            {
                _logger.LogError(LogMessage.RecordNotExist.ToFormattedString(), ErrorCodes.RecordNotFound.GetDescription(), "ContactFactory-DeleteAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            }
            oldData.IsDeleted = true;
            _context.Contacts.Update(oldData);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagingResponse<Contact>> GetAllAsync(PagingRequest request)
        {
            var query = _context.Contacts
                .Include(x=>x.User)
                .Include(x => x.ContactUser)
                .Where(x => !x.IsDeleted && x.UserId==userId).OrderBy(x=>x.ContactUser.FirstName)
                .AsQueryable();

            return new PagingResponse<Contact>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
        }

        public Task<Contact?> GetAsync(int id)
        {
            return _context.Contacts
                .Where(x => !x.IsDeleted && x.Id == id && x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<PagingResponse<Contact>> SearchAsync(SearchRequest request)
        {
            var query = _context.Contacts
                .Include(x => x.User)
                .Include(x => x.ContactUser)
                .Where(x => !x.IsDeleted)
                .Where(x => x.ContactUser.FirstName.Contains(request.SearchTerm) ||
                            x.ContactUser.Email.Contains(request.SearchTerm) ||
                            x.ContactUser.Phone.Contains(request.SearchTerm) ||
                            x.ContactUser.LastName.Contains(request.SearchTerm))
                .OrderBy(x => x.ContactUser.FirstName)
                .AsQueryable();

            return new PagingResponse<Contact>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
        }

        public async Task<List<User>> SearchUser(string search)
        {
            return await _context.Users
                .Where(x => !x.IsDeleted)
                .Where(x => x.FirstName.Contains(search) ||
                            x.Email.Contains(search) ||
                            x.Phone.Contains(search) ||
                            x.LastName.Contains(search))
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        public Task<int> UpdateAsync(Contact request)
        {
            throw new NotImplementedException();
        }
    }
}
