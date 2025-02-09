using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class ExpenseShareResponse: BaseResponseModel
    {
        public List<ExpenseShareResponse> ExpenseShares { get; set; }
    }
}
