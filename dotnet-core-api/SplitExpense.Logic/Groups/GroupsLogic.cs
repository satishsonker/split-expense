using AutoMapper;
using SplitExpense.Data.DbModels;
using SplitExpense.Data.Factory;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public class GroupsLogic(IMapper mapper,IGroupFactory factory) : IGroupLogic
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGroupFactory _factory=factory;

        public async Task<GroupResponse> CreateAsync(GroupRequest request)
        {
            var mappedRequest = _mapper.Map<Group>(request);
            return _mapper.Map<GroupResponse>(await _factory.CreateAsync(mappedRequest));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _factory.DeleteAsync(id);
        }

        public async Task<PagingResponse<UserGroupMappingResponse>> GetAllAsync(PagingRequest request)
        {
            return _mapper.Map<PagingResponse<UserGroupMappingResponse>>(await  _factory.GetAllAsync(request));
        }

        public async Task<GroupResponse> GetAsync(int id)
        {
            return _mapper.Map<GroupResponse>(await _factory.GetAsync(id));
        }

        public async Task<PagingResponse<UserGroupMappingResponse>> SearchAsync(SearchRequest request)
        {
            return _mapper.Map<PagingResponse<UserGroupMappingResponse>>(await _factory.SearchAsync(request));
        }

        public async Task<int> UpdateAsync(GroupRequest request)
        {
            var mappedRequest = _mapper.Map<Group>(request);
            return await _factory.UpdateAsync(mappedRequest);
        }
    }
}
