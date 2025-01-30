namespace SplitExpense.Models.Common
{
    public class PagingResponse<T> : BasePagingModel where T : class
    {
        public List<T> Data { get; set; }
    }
}
