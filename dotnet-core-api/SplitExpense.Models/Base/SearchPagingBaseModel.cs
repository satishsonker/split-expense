namespace SplitExpense.Models
{
    public class SearchPagingBaseModel:PagingBaseModel
    {
        public virtual string? SearchTerm { get; set; }
        public virtual DateOnly? FromDate { get; set; }
        public virtual DateOnly? ToDate { get; set; }
    }
}
