namespace SplitExpense.Models
{
    public abstract class BasePagingRequestModel
    {
        public virtual int PageNo { get; set; } = 1;
        public virtual int PageSize { get; set; } = 20;
        public virtual DateTime? FromDate { get; set; }
        public virtual DateTime? ToDate { get; set; }
    }
}
