namespace SplitExpense.Models.Base
{
    public class BasePagingResponseModel
    {
        public virtual int PageNo { get; set; } = 1;
        public virtual int PageSize { get; set; } = 20;
        public int RecordCounts { get; set; }
    }
}
