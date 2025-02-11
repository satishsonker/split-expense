using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [ApiController]
    public class ContactsController(IContactLogic contactLogic) : ControllerBase
    {
        private readonly IContactLogic _contactLogic=contactLogic;

        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.ContactCreate)]
        public async Task<ContactResponse> CreateAsync([FromBody] UserRequest request)
        {
            return await _contactLogic.CreateAsync(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.ContactAddInList)]
        public async Task<bool> AddInContactListAsync([FromRoute] int id)
        {
            return await _contactLogic.AddInContactListAsync(id);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete(ApiPaths.ContactDelete)]
        public async Task<bool> DeleteAsync([FromRoute] int id)
        {
            return await _contactLogic.DeleteAsync(id);
        }


        [ProducesResponseType(typeof(PagingResponse<ContactResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.ContactGetAll)]
        public async Task<PagingResponse<ContactResponse>> GetAllAsync([FromQuery]PagingRequest request)
        {
            return await _contactLogic.GetAllAsync(request);
        }

        [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.ContactGetById)]
        public async Task<ContactResponse> GetAsync([FromRoute]int id)
        {
           return await _contactLogic.GetAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<ContactResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.ContactSearch)]
        public async Task<PagingResponse<ContactResponse>> SearchAsync([FromQuery]SearchRequest request)
        {
            return await _contactLogic.SearchAsync(request);
        }

        [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.ContactSearchUser)]
        public async Task<List<UserResponse>> SearchUserAsync([FromQuery] string searchTerm)
        {
            return await _contactLogic.SearchUser(searchTerm);
        }

        [ProducesResponseType(typeof(PagingResponse<ContactResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut(ApiPaths.ContactUpdate)]
        public async Task<int> UpdateAsync([FromBody]ContactRequest request)
        {
           return await _contactLogic.UpdateAsync(request);
        }
    }
}
