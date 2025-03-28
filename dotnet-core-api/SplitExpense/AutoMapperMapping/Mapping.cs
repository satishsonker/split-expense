﻿using AutoMapper;
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
                CreateMap<GroupRequest, ExpenseGroup>()
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.ThumbImagePath, opt => opt.Ignore());
            CreateMap<ExpenseGroup, GroupResponse>();
            CreateMap<GroupDetailRequest, GroupDetail>();
            CreateMap<GroupDetail, GroupDetailResponse>();
                CreateMap<PagingResponse<ExpenseGroup>, PagingResponse<GroupResponse>>();
            #endregion

            #region User Group Mapping
                CreateMap<UserGroupMapping, UserGroupMappingResponse>()
                   .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
                    .ForMember(dest => dest.AddedByUser, opt => opt.MapFrom(src => $"{src.Group.User.FirstName} {src.Group.User.LastName}"))
                     .ForMember(dest => dest.AddedUserId, opt => opt.MapFrom(src => src.FriendId))
                      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Group.CreatedAt))
                       .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.CreatedAt))
                       // .ForMember(dest => dest.AddedUser, opt => opt.MapFrom(src => $"{src.AddedUser.FirstName} {src.AddedUser.LastName}"))
                         .ForMember(dest => dest.AddedByUserId, opt => opt.MapFrom(src => src.CreatedBy));
                CreateMap<PagingResponse<UserGroupMapping>, PagingResponse<UserGroupMappingResponse>>();
            #endregion

            #region Split Type
                CreateMap<SplitTypeRequest, SplitType>();
                CreateMap<SplitType, SplitTypeResponse>();
                CreateMap<PagingResponse<SplitType>, PagingResponse<SplitTypeResponse>>();
            #endregion

            #region Expense
                CreateMap<ExpenseRequest, Expense>();
                CreateMap<Expense, ExpenseResponse>();

                CreateMap<ExpenseShareRequest, ExpenseShare>();
                CreateMap<ExpenseShare, ExpenseShareResponse>();
            #endregion

            #region User
            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>();
            #endregion

            #region Contact
            CreateMap<ContactRequest, Contact>();
            CreateMap<Contact, ContactResponse>();
            CreateMap<PagingResponse<Contact>, PagingResponse<ContactResponse>>();
            #endregion

            #region GroupType
            CreateMap<GroupTypeRequest, GroupType>();
            CreateMap<GroupType, GroupTypeResponse>();
            CreateMap<PagingResponse<GroupType>, PagingResponse<GroupTypeResponse>>();
            #endregion

            #region Auth
            CreateMap<User, LoginResponse>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.TokenExpiration, opt => opt.Ignore());
            #endregion

            #region ExpenseActivity
            CreateMap<ExpenseActivity, ExpenseActivityResponse>();
            CreateMap<PagingResponse<ExpenseActivity>, PagingResponse<ExpenseActivityResponse>>();
            #endregion

            #region Activity
            CreateMap<ExpenseActivityRequest, ExpenseActivity>();
            CreateMap<ExpenseActivity, ExpenseActivityResponse>();
            CreateMap<PagingResponse<ExpenseActivity>, PagingResponse<ExpenseActivityResponse>>();
            #endregion
        }
    }
}
