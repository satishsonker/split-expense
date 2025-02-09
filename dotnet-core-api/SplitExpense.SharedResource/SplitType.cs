namespace SplitExpense.SharedResource
{
    public enum SplitType
    {
        Equal,        // Divide equally
        Unequal,      // Custom amounts per user
        Percentage,   // Split based on percentage
        ExactAmount,  // Fixed amount per user
        Shares,       // Split based on shares (e.g., rent)
        Adjustment
    }
}
