using AutoMapper;
using SplitExpense.Data.DbModels;
using SplitExpense.Models;

namespace SplitExpense.AutoMapperMapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GroupRequest, Group>();
            CreateMap<Group, GroupResponse>();
        }
    }
}
