using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public class ExpenseLogic(IExpenseFactory expenseFactory,IMapper mapper) : IExpenseLogic
    {
        private readonly IMapper _mapper=mapper;
        private readonly IExpenseFactory _expenseFactory = expenseFactory;
        public async Task<ExpenseResponse> AddExpense(ExpenseRequest request)
        {
            var mappedRequest=_mapper.Map<Expense>(request);
            return _mapper.Map<ExpenseResponse>(await _expenseFactory.AddExpense(mappedRequest));
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            return await _expenseFactory.DeleteExpense(expenseId);
        }

        public async Task<ExpenseResponse> UpdateExpense(ExpenseRequest request)
        {
            var mappedRequest = _mapper.Map<Expense>(request);
            return _mapper.Map<ExpenseResponse>(await _expenseFactory.UpdateExpense(mappedRequest));
        }

        public async Task<PagingResponse<ExpenseResponse>> GetAllAsync(PagingRequest request)
        {
            var result = await _expenseFactory.GetAllAsync(request);
            return _mapper.Map<PagingResponse<ExpenseResponse>>(result);
        }

        public async Task<PagingResponse<ExpenseResponse>> SearchAsync(SearchRequest request)
        {
            var result = await _expenseFactory.SearchAsync(request);
            return _mapper.Map<PagingResponse<ExpenseResponse>>(result);
        }
    }
}
