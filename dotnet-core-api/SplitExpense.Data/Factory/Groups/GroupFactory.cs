using Microsoft.EntityFrameworkCore;
using SplitExpense.Data.DbModels;
using SplitExpense.Middleware.Exceptions;
using SplitExpense.Models;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory
{
    public class GroupFactory(SplitExpenseDbContext context) : IGroupFactory
    {
        private SplitExpenseDbContext _context = context;

        public async Task<Group> CreateAsync(Group request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                var trans =await _context.Database.BeginTransactionAsync();
                var entity =await _context.Groups.AddAsync(request);
                if (await _context.SaveChangesAsync()>0)
                {
                    var userGroupMap = new UserGroupMapping()
                    {
                        GroupId = entity.Entity.Id
                    };
                    _context.UserGroupMappings.Add(userGroupMap);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        await trans.CommitAsync();
                        return entity.Entity;
                    }
                   //throw new BusinessRuleViolationException(ErrorCodes.UnableToAddRecord);
                }
                throw new DbUpdateException();
            }
            catch(Exception ex)
            {
                throw new DbUpdateException(ex.Message);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Group>> GetAllAsync(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Group> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Group>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Group request)
        {
            throw new NotImplementedException();
        }
    }
}
