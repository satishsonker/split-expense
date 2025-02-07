using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic.Masters
{
    public class SplitTypeLogic(ISplitTypeFactory splitTypeFactory, IMapper mapper, ISplitExpenseLogger logger) : ISplitTypeLogic
    {
        private readonly IMapper _mapper = mapper;
        private readonly ISplitTypeFactory _splitTypeFactory = splitTypeFactory;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<SplitTypeResponse> CreateAsync(SplitTypeRequest request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request, nameof(request));

                var splitType = _mapper.Map<SplitType>(request);
                var result = await _splitTypeFactory.CreateAsync(splitType);
                return _mapper.Map<SplitTypeResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating split type");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid id", nameof(id));
                }

                return await _splitTypeFactory.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting split type with id {id}");
                throw;
            }
        }

        public async Task<PagingResponse<SplitTypeResponse>> GetAllAsync(PagingRequest request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request, nameof(request));

                var result = await _splitTypeFactory.GetAllAsync(request);
               
                return _mapper.Map<PagingResponse<SplitTypeResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all split types");
                throw;
            }
        }

        public async Task<SplitTypeResponse> GetAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid id", nameof(id));
                }

                var result = await _splitTypeFactory.GetAsync(id);
                return _mapper.Map<SplitTypeResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting split type with id {id}");
                throw;
            }
        }

        public async Task<PagingResponse<SplitTypeResponse>> SearchAsync(SearchRequest request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request, nameof(request));

                var result = await _splitTypeFactory.SearchAsync(request);

                return _mapper.Map<PagingResponse<SplitTypeResponse>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching split types");
                throw;
            }
        }

        public async Task<int> UpdateAsync(SplitTypeRequest request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request, nameof(request));

                var splitType = _mapper.Map<SplitType>(request);
                return await _splitTypeFactory.UpdateAsync(splitType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating split type");
                throw;
            }
        }
    }
}
