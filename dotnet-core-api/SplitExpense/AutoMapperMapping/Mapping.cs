using AutoMapper;
using SplitExpense.Data.DbModels;
using SplitExpense.Models;
using SplitExpense.Models.Common;
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
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => $"{src.Group.User.FirstName} {src.Group.User.LastName}"))
                 .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.UserId))
                  .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Group.CreatedAt))
                   .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.AddedByUser, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                     .ForMember(dest => dest.AddedByUserId, opt => opt.MapFrom(src => src.CreatedBy));
            CreateMap<PagingResponse<UserGroupMapping>, PagingResponse<UserGroupMappingResponse>>();
            #endregion
        }
    }
}
