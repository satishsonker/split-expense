namespace SplitExpense.Models
{
    public class BaseSearchPagingModel:BasePagingModel
    {
        public virtual string? SearchTerm { get; set; }
        public virtual DateOnly? FromDate { get; set; }
        public virtual DateOnly? ToDate { get; set; }
    }
}
