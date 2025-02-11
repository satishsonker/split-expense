using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public class ContactLogic(IContactFactory contactFactory, IMapper mapper) : IContactLogic
    {
        private readonly IContactFactory _contactFactory = contactFactory;
        private readonly IMapper _mapper = mapper;
        public async Task<ContactResponse> CreateAsync(UserRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var contact = _mapper.Map<User>(request);
            return _mapper.Map<ContactResponse>(await _contactFactory.CreateAsync(contact));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _contactFactory.DeleteAsync(id);
        }

        public async Task<PagingResponse<ContactResponse>> GetAllAsync(PagingRequest request)
        {
            return _mapper.Map<PagingResponse<ContactResponse>>(await _contactFactory.GetAllAsync(request));

        }

        public async Task<ContactResponse> GetAsync(int id)
        {
            return _mapper.Map<ContactResponse>(await _contactFactory.GetAsync(id));
        }

        public async Task<PagingResponse<ContactResponse>> SearchAsync(SearchRequest request)
        {
            return _mapper.Map<PagingResponse<ContactResponse>>(await _contactFactory.SearchAsync(request));
        }

        public async Task<List<UserResponse>> SearchUser(string search)
        {
            return _mapper.Map<List<UserResponse>>(await _contactFactory.SearchUser(search));
        }

        public async Task<int> UpdateAsync(ContactRequest request)
        {
            var contact = _mapper.Map<Contact>(request);
            return await _contactFactory.UpdateAsync(contact);
        }
    }
}
