namespace SplitExpense.Models
{
    public abstract class PagingBaseModel
    {
        public virtual int PageNo { get; set; } = 1;
        public virtual int PageSize { get; set; } = 20;
        public virtual int RecordCounts { get; set; }
    }
}
