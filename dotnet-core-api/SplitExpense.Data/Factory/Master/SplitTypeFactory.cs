using Microsoft.EntityFrameworkCore;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;


namespace SplitExpense.Data.Factory
{
    public class SplitTypeFactory(SplitExpenseDbContext context) : ISplitTypeFactory
    {
        private readonly SplitExpenseDbContext _context=context;
        public async Task<SplitType> CreateAsync(SplitType request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await _context.SplitTypes.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var splitType = await _context.SplitTypes.FindAsync(id);
            if (splitType == null) return false;
            
            splitType.IsDeleted = true;
            _context.SplitTypes.Update(splitType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagingResponse<SplitType>> GetAllAsync(PagingRequest request)
        {
            var query = _context.SplitTypes.Where(x => !x.IsDeleted);
            
            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagingResponse<SplitType>
            {
                Data = items,
                RecordCounts = totalRecords,
                PageNo = request.PageNo,
                PageSize = request.PageSize
            };
        }

        public async Task<SplitType?> GetAsync(int id)
        {
            return await _context.SplitTypes
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PagingResponse<SplitType>> SearchAsync(SearchRequest request)
        {
            var query = _context.SplitTypes
                .Where(x => !x.IsDeleted && 
                    (string.IsNullOrEmpty(request.SearchTerm) || 
                     x.Name.Contains(request.SearchTerm) ||
                     x.Description.Contains(request.SearchTerm)));

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagingResponse<SplitType>
            {
                Data = items,
                RecordCounts = totalRecords,
                PageNo = request.PageNo,
                PageSize = request.PageSize
            };
        }

        public async Task<int> UpdateAsync(SplitType request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var existingSplitType = await _context.SplitTypes
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (existingSplitType == null)
                return 0;

            existingSplitType.Name = request.Name;
            existingSplitType.Description = request.Description;
            existingSplitType.UpdatedAt = DateTime.UtcNow;
            existingSplitType.UpdatedBy = request.UpdatedBy;

            _context.SplitTypes.Update(existingSplitType);
            return await _context.SaveChangesAsync();
        }
    }
}
