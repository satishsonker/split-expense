using AutoMapper;
using SplitExpense.Data.DbModels;
using SplitExpense.Data.Factory;
using SplitExpense.Models;

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

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GroupResponse>> GetAllAsync(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GroupResponse> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GroupResponse>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(GroupRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
