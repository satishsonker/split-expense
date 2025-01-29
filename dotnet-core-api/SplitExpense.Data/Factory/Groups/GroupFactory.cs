using Microsoft.EntityFrameworkCore;
using SplitExpense.Data.DbModels;
using SplitExpense.Models;

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

                var entity =await _context.Groups.AddAsync(request);
                if (await _context.SaveChangesAsync()>0)
                {
                    return entity.Entity;
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
