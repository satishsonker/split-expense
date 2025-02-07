namespace SplitExpense.Models
{
    public class BaseSearchPagingModel:BasePagingRequestModel
    {
        public virtual string? SearchTerm { get; set; }
    }
}
