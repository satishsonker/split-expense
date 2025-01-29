using System.ComponentModel;

namespace SplitExpense.SharedResource
{
    public enum ErrorCodes
    {
        #region Crud Related Codes
        [Description("Unable to add record in database")]
        UnableToAddRecord = 10000
        #endregion
    }
}
