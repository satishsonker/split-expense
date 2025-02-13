using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Logic.Email;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.Logger;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.SharedResource;
using SplitExpense.FileManagement.Service;

namespace SplitExpense.Logic
{
    public class GroupsLogic(IMapper mapper, IGroupFactory factory,IEmailLogic emailLogic,ISplitExpenseLogger logger, IFileUploadService fileUploadService) : IGroupLogic
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGroupFactory _factory=factory;
        private readonly IEmailLogic _emailLogic=emailLogic;
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IFileUploadService _fileUploadService = fileUploadService;

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
                        { "addedByUserEmail", groupMappingData.AddedUser.Email }
                    };
                    await _emailLogic.SendEmailOnUserAddedInGroup(groupMappingData.AddedUser.Email,
                          groupMappingData.AddedByUser,
                          $"{groupMappingData.AddedUser.FirstName} {groupMappingData.AddedUser.LastName}",
                          groupMappingData.CreatedAt, data);
                }
            }
            return res?.Id>0;
        }

        public async Task<GroupResponse> CreateAsync(GroupRequest request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                var mappedRequest = _mapper.Map<Group>(request);
                if(request.GroupImage != null && request.GroupImage.Length>0)
                {
                    var fileUploadResponse = await _fileUploadService.UploadFileAsync(request.GroupImage);
                    mappedRequest.ImagePath = fileUploadResponse.FilePath;
                    mappedRequest.ThumbImagePath = fileUploadResponse.ThumbnailPath;
                }
                return _mapper.Map<GroupResponse>(await _factory.CreateAsync(mappedRequest, request.Members));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _factory.DeleteAsync(id);
        }

        public async Task<PagingResponse<GroupResponse>> GetAllAsync(PagingRequest request)
        {
            try
            {
                if (request.PageSize <= 0)
                    throw new ArgumentException("InvalidPageSize");
                if (request.PageNo <= 0)
                    throw new ArgumentException("InvalidPageNo");
                var res = _mapper.Map<PagingResponse<GroupResponse>>(await _factory.GetAllAsync(request));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, LogMessage.GeneralError);
                throw new BusinessRuleViolationException((int)ErrorCodes.GeneralError,ex.Message,null);
            }
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
