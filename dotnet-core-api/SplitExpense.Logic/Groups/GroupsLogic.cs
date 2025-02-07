using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Logic.Email;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public class GroupsLogic(IMapper mapper, IGroupFactory factory,IEmailLogic emailLogic) : IGroupLogic
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGroupFactory _factory=factory;
        private readonly IEmailLogic _emailLogic=emailLogic;

        public async Task<bool> AddFriendInGroupAsync(AddFriendInGroupRequest request)
        {
            var res=await _factory.AddFriendInGroupAsync(request);
            if(res!=null)
            {
                var groupMappingData = await GetUserGroupMappingAsync(res.Id);
                if (groupMappingData != null)
                {
                    var data = new Dictionary<string, string>
                    {
                        { "groupName", groupMappingData.GroupName },
                        { "addedByUserEmail", groupMappingData.AddedByUserEmail }
                    };
                  await  _emailLogic.SendEmailOnUserAddedInGroup(groupMappingData.AddedUserEmail, 
                        groupMappingData.AddedByUser, 
                        groupMappingData.AddedUser, 
                        groupMappingData.CreatedAt,data);
                }
            }
            return res?.Id>0;
        }

        public async Task<GroupResponse> CreateAsync(GroupRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var mappedRequest = _mapper.Map<Group>(request);
            return _mapper.Map<GroupResponse>(await _factory.CreateAsync(mappedRequest));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _factory.DeleteAsync(id);
        }

        public async Task<PagingResponse<UserGroupMappingResponse>> GetAllAsync(PagingRequest request)
        {
            if(request.PageSize<=0)
            throw new ArgumentException("InvalidPageSize");
            if (request.PageNo <= 0)
                throw new ArgumentException("InvalidPageNo");
            return _mapper.Map<PagingResponse<UserGroupMappingResponse>>(await  _factory.GetAllAsync(request));
        }

        public async Task<GroupResponse> GetAsync(int id)
        {
            return _mapper.Map<GroupResponse>(await _factory.GetAsync(id));
        }

        public async Task<UserGroupMappingResponse?> GetUserGroupMappingAsync(int id)
        {
            return _mapper.Map<UserGroupMappingResponse>(await _factory.GetUserGroupMappingAsync(id));
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
