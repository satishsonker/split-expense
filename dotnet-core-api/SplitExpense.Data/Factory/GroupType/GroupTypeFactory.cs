using Microsoft.EntityFrameworkCore;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Logger;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory
{
    public class GroupTypeFactory : IGroupTypeFactory
    {
        private readonly SplitExpenseDbContext _context;
        private readonly ISplitExpenseLogger _logger;

        public GroupTypeFactory(SplitExpenseDbContext context, ISplitExpenseLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GroupType> CreateAsync(GroupType request)
        {
            var oldData = await _context.GroupTypes
                .Where(x => !x.IsDeleted && x.Name.ToLower() == request.Name.ToLower())
                .FirstOrDefaultAsync();

            if (oldData != null)
            {
                _logger.LogError(LogMessage.RecordAlreadyExist.ToFormattedString(), ErrorCodes.RecordAlreadyExist.GetDescription(), "GroupTypeFactory-CreateAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordAlreadyExist);
            }

            var entity = await _context.GroupTypes.AddAsync(request);
            await _context.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var oldData = await _context.GroupTypes
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync();

            if (oldData == null)
            {
                _logger.LogError(LogMessage.RecordNotExist.ToFormattedString(), ErrorCodes.RecordNotFound.GetDescription(), "GroupTypeFactory-DeleteAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            }

            oldData.IsDeleted = true;
            _context.GroupTypes.Update(oldData);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagingResponse<GroupType>> GetAllAsync(PagingRequest request)
        {
            var query = _context.GroupTypes
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .AsQueryable();

            return new PagingResponse<GroupType>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
        }

        public async Task<GroupType?> GetAsync(int id)
        {
            return await _context.GroupTypes
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PagingResponse<GroupType>> SearchAsync(SearchRequest request)
        {
            var query = _context.GroupTypes
                .Where(x => !x.IsDeleted && x.Name.Contains(request.SearchTerm))
                .OrderBy(x => x.Name)
                .AsQueryable();

            return new PagingResponse<GroupType>()
            {
                Data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                RecordCounts = await query.CountAsync(),
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
        }

        public async Task<int> UpdateAsync(GroupType request)
        {
            var oldData = await _context.GroupTypes
                .Where(x => !x.IsDeleted && x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (oldData == null)
            {
                _logger.LogError(LogMessage.RecordNotExist.ToFormattedString(), ErrorCodes.RecordNotFound.GetDescription(), "GroupTypeFactory-UpdateAsync");
                throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            }

            oldData.Name = request.Name;
            oldData.Description = request.Description;
            _context.GroupTypes.Update(oldData);
            return await _context.SaveChangesAsync();
        }
    }
} 