using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.Models.DTO.Response;

namespace SplitExpense.Logic
{
    public class GroupTypeLogic : IGroupTypeLogic
    {
        private readonly IGroupTypeFactory _groupTypeFactory;
        private readonly IMapper _mapper;
        private readonly ISplitExpenseLogger _logger;

        public GroupTypeLogic(IGroupTypeFactory groupTypeFactory, IMapper mapper, ISplitExpenseLogger logger)
        {
            _groupTypeFactory = groupTypeFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GroupTypeResponse> CreateAsync(GroupTypeRequest request)
        {
            var entity = _mapper.Map<GroupType>(request);
            var result = await _groupTypeFactory.CreateAsync(entity);
            return _mapper.Map<GroupTypeResponse>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _groupTypeFactory.DeleteAsync(id);
        }

        public async Task<PagingResponse<GroupTypeResponse>> GetAllAsync(PagingRequest request)
        {
            var result = await _groupTypeFactory.GetAllAsync(request);
            return _mapper.Map<PagingResponse<GroupTypeResponse>>(result);
        }

        public async Task<GroupTypeResponse?> GetAsync(int id)
        {
            var result = await _groupTypeFactory.GetAsync(id);
            return _mapper.Map<GroupTypeResponse>(result);
        }

        public async Task<PagingResponse<GroupTypeResponse>> SearchAsync(SearchRequest request)
        {
            var result = await _groupTypeFactory.SearchAsync(request);
            return _mapper.Map<PagingResponse<GroupTypeResponse>>(result);
        }

        public async Task<int> UpdateAsync(GroupTypeRequest request)
        {
            var entity = _mapper.Map<GroupType>(request);
            return await _groupTypeFactory.UpdateAsync(entity);
        }
    }
} 