using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Data.Services;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.FileManagement.Service;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Logic
{
    public class GroupsLogic(IMapper mapper, 
        IGroupFactory factory,
        IEmailLogic emailLogic,
        ISplitExpenseLogger logger, 
        IFileUploadService fileUploadService,IExpenseActivityLogic expenseActivityLogic,IUserContextService userContextService) : IGroupLogic
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGroupFactory _factory=factory;
        private readonly IEmailLogic _emailLogic=emailLogic;
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IFileUploadService _fileUploadService = fileUploadService;
        private readonly IExpenseActivityLogic _expenseActivityLogic = expenseActivityLogic;
        private readonly IUserContextService _userContextService = userContextService;

        public async Task<bool> AddFriendInGroupAsync(AddFriendInGroupRequest request)
        {
            var res=await _factory.AddFriendInGroupAsync(request);
            if (res.Count==0) return false;
            var ids = res.Select(x => x.Id).ToList();

            var groupMappings = await GetUserGroupMappingAsync(ids);
            groupMappings.ForEach(groupMappingData =>
            {
                if (groupMappingData != null)
                {
                    var data = new Dictionary<string, string>
                    {
                        { "groupName", groupMappingData.GroupName },
                        { "addedByUserEmail", groupMappingData.AddedUser.Email }
                    };
                    _emailLogic.SendEmailOnUserAddedInGroup(groupMappingData.AddedUser.Email,
                          groupMappingData.AddedByUser,
                          $"{groupMappingData.AddedUser.FirstName} {groupMappingData.AddedUser.LastName}",
                          groupMappingData.CreatedAt, data);
                    CreateAddMemberInGroupActivity(groupMappingData);
                }
            });
            return res.Count>0;
        }

        private async Task CreateAddMemberInGroupActivity(UserGroupMappingResponse? groupMappingData)
        {
            List<ExpenseActivityRequest> activityList = [];
            var addinGroupData = new Dictionary<string, string>
                    {
                        { "adder", "You" },
                        { "addedUser", groupMappingData.AddedUser.FirstName+" "+groupMappingData.AddedUser.LastName },
                        { "groupName", groupMappingData.GroupName }
                    };
            var addinGroup = _expenseActivityLogic.GetActivityMessage(ExpenseActivityTypeEnum.MemberAddedInGroup, addinGroupData);
            activityList.Add(new ExpenseActivityRequest
            {
                ActivityType = (int)ExpenseActivityTypeEnum.MemberAddedInGroup,
                Activity = addinGroup,
                UserId = groupMappingData.AddedByUserId
            });

            var addedByInGroupData = new Dictionary<string, string>
                    {
                        { "addedBy", groupMappingData.AddedByUser },
                        { "addedUser", "You" },
                        { "groupName", groupMappingData.GroupName }
                    };
            var addedByInGroup = _expenseActivityLogic.GetActivityMessage(ExpenseActivityTypeEnum.MemberAddedByGroupMember, addedByInGroupData);
            activityList.Add(new ExpenseActivityRequest
            {
                ActivityType = (int)ExpenseActivityTypeEnum.MemberAddedInGroup,
                Activity = addedByInGroup,
                UserId = groupMappingData.AddedByUserId
            });
            activityList.Add(new ExpenseActivityRequest
            {
                ActivityType = (int)ExpenseActivityTypeEnum.MemberAddedByGroupMember,
                Activity = addinGroup,
                UserId = groupMappingData.AddedUserId
            });
            await _expenseActivityLogic.CreateRangeAsync(activityList);
        }

        public async Task<GroupResponse> CreateAsync(GroupRequest request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);
                var mappedRequest = _mapper.Map<ExpenseGroup>(request);
                if(request.GroupImage != null && request.GroupImage.Length>0)
                {
                    var fileUploadResponse = await _fileUploadService.UploadFileAsync(request.GroupImage);
                    mappedRequest.ImagePath = fileUploadResponse.FilePath;
                    mappedRequest.ThumbImagePath = fileUploadResponse.ThumbnailPath;
                }
                var response= _mapper.Map<GroupResponse>(await _factory.CreateAsync(mappedRequest,request.Members));
                var addinGroupData = new Dictionary<string, string>
                    {
                        { "creator", "You" },
                        { "groupName", request.Name }
                    };
                var addinGroup = _expenseActivityLogic.GetActivityMessage(ExpenseActivityTypeEnum.GroupCreated, addinGroupData);
                List<ExpenseActivityRequest> expenseActivities = [];
                expenseActivities.Add(new ExpenseActivityRequest
                {
                    ActivityType = (int)ExpenseActivityTypeEnum.MemberAddedInGroup,
                    Activity = addinGroup,
                    UserId = _userContextService.GetUserId()
                });

                if(request.Members.Count != 0)
                {
                    request.Members.ForEach(member =>
                    {
                        var addinGroupData = new Dictionary<string, string>
                        {
                            { "creator", "You" },
                            { "groupName", request.Name }
                        };
                        var addinGroup = _expenseActivityLogic.GetActivityMessage(ExpenseActivityTypeEnum.GroupCreated, addinGroupData);
                        expenseActivities.Add(new ExpenseActivityRequest
                        {
                            ActivityType = (int)ExpenseActivityTypeEnum.MemberAddedInGroup,
                            Activity = addinGroup,
                            UserId = member
                        });
                    });
                }
                await _expenseActivityLogic.CreateRangeAsync(expenseActivities);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, LogMessage.GeneralError);
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

        public async Task<List<GroupResponse>> GetRecentGroups()
        {
            return _mapper.Map<List<GroupResponse>>(await _factory.GetRecentGroups());
        }

        public async Task<List<UserGroupMappingResponse>> GetUserGroupMappingAsync(List<int> ids)
        {
            return _mapper.Map<List<UserGroupMappingResponse>>(await _factory.GetUserGroupMappingAsync(ids));
        }

        public async Task<PagingResponse<UserGroupMappingResponse>> SearchAsync(SearchRequest request)
        {
            return _mapper.Map<PagingResponse<UserGroupMappingResponse>>(await _factory.SearchAsync(request));
        }

        public async Task<int> UpdateAsync(GroupRequest request)
        {
            var mappedRequest = _mapper.Map<ExpenseGroup>(request);
            if (request.GroupImage != null && request.GroupImage.Length > 0)
            {
                var fileUploadResponse = await _fileUploadService.UploadFileAsync(request.GroupImage);
                mappedRequest.ImagePath = fileUploadResponse.FilePath;
                mappedRequest.ThumbImagePath = fileUploadResponse.ThumbnailPath;
            }
            return await _factory.UpdateAsync(mappedRequest,request.Members);
        }
    }
}
