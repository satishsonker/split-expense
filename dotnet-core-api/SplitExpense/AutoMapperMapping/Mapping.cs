using AutoMapper;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.AutoMapperMapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            #region Group
            CreateMap<GroupRequest, Group>();
            CreateMap<Group, GroupResponse>();
            CreateMap<PagingResponse<Group>, PagingResponse<GroupResponse>>();
            #endregion

            #region User Group Mapping
            CreateMap<UserGroupMapping, UserGroupMappingResponse>()
               .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
                .ForMember(dest => dest.AddedByUser, opt => opt.MapFrom(src => $"{src.Group.User.FirstName} {src.Group.User.LastName}"))
                 .ForMember(dest => dest.AddedUserId, opt => opt.MapFrom(src => src.FriendId))
                  .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Group.CreatedAt))
                   .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.AddedUser, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                    .ForMember(dest => dest.AddedUserEmail, opt => opt.MapFrom(src => src.User.Email))
                     .ForMember(dest => dest.AddedByUserEmail, opt => opt.MapFrom(src => src.AddedByUser.Email))
                     .ForMember(dest => dest.AddedByUserId, opt => opt.MapFrom(src => src.CreatedBy));
            CreateMap<PagingResponse<UserGroupMapping>, PagingResponse<UserGroupMappingResponse>>();
            #endregion

            #region Split Type
            CreateMap<SplitTypeRequest, SplitType>();
            CreateMap<SplitType, SplitTypeResponse>();
            CreateMap<PagingResponse<SplitType>, PagingResponse<SplitTypeResponse>>();
            #endregion
        }
    }
}
