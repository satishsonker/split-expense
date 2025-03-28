﻿using NLog.Config;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IGroupLogic
    {
        Task<GroupResponse> CreateAsync(GroupRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagingResponse<GroupResponse>> GetAllAsync(PagingRequest request);
        Task<GroupResponse> GetAsync(int id);
        Task<PagingResponse<UserGroupMappingResponse>> SearchAsync(SearchRequest request);
        Task<int> UpdateAsync(GroupRequest request);
        Task<bool> AddFriendInGroupAsync(AddFriendInGroupRequest request);
        Task<List<UserGroupMappingResponse>> GetUserGroupMappingAsync(List<int> ids);
        Task<List<GroupResponse>> GetRecentGroups();
    }
}
