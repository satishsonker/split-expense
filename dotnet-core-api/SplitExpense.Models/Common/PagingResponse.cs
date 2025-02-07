using SplitExpense.Models.Base;

namespace SplitExpense.Models.Common
{
    public class PagingResponse<T> : BasePagingResponseModel where T : class
    {
        public List<T> Data { get; set; }
    }
}
