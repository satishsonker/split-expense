using SplitExpense.Data.DbModels;
using SplitExpense.Models;

namespace SplitExpense.Data.Factory
{
    public class GroupFactory(SplitExpenseDbContext context) : IGroupFactory
    {
        private SplitExpenseDbContext _context = context;

        public Task<Group> CreateAsync(Group request)
        {
            try
            {
                if(request == null)
                    throw new ArgumentNullException(nameof(request));
            }
            catch(Exception ex)
            {

            }
            finally
            {

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
