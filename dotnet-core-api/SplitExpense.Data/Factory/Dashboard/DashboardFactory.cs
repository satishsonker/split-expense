using Microsoft.EntityFrameworkCore;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Data.Factory
{
    public class DashboardFactory(SplitExpenseDbContext context) : IDashboardFactory
    {
        private readonly SplitExpenseDbContext _context = context;

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int userId)
        {
            try
            {
                // Get all expenses where user is involved (either paid by user or user has a share)
                var expenses = await _context.Expenses
                    .Where(x => !x.IsDeleted &&
                        (x.PaidByUserId == userId ||
                         x.ExpenseShares.Any(share => share.UserId == userId && !share.IsDeleted)))
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .ToListAsync();

                decimal youOwe = 0;
                decimal youAreOwed = 0;

                foreach (var expense in expenses)
                {
                    var paidByMe = expense.PaidByUserId == userId;

                    if (paidByMe)
                    {
                        // I paid for this expense - I'm owed what others owe me
                        if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                        {
                            var totalOwedByOthers = expense.ExpenseShares
                                .Where(share => share.UserId != userId)
                                .Sum(share => share.AmountOwed);
                            youAreOwed += totalOwedByOthers;
                        }
                    }
                    else
                    {
                        // Someone else paid - check if I owe anything
                        if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                        {
                            var myShare = expense.ExpenseShares.FirstOrDefault(share => share.UserId == userId);
                            if (myShare != null)
                            {
                                youOwe += myShare.AmountOwed;
                            }
                        }
                    }
                }

                var totalBalance = youAreOwed - youOwe;

                return new DashboardSummaryResponse
                {
                    TotalBalance = totalBalance,
                    YouOwe = youOwe,
                    YouAreOwed = youAreOwed
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting dashboard summary", ex);
            }
        }

        public async Task<PagingResponse<Expense>> GetExpensesYouOweAsync(int userId, PagingRequest request)
        {
            try
            {
                // Get expenses where someone else paid and user has a share
                var query = _context.Expenses
                    .Where(x => !x.IsDeleted &&
                        x.PaidByUserId != userId &&
                        x.ExpenseShares.Any(share => share.UserId == userId && !share.IsDeleted))
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .Include(x => x.PaidByUser)
                    .Include(x => x.Group)
                    .OrderByDescending(x => x.ExpenseDate)
                    .ThenByDescending(x => x.CreatedAt)
                    .AsQueryable();

                return new PagingResponse<Expense>
                {
                    Data = await query
                        .Skip((request.PageNo - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync(),
                    RecordCounts = await query.CountAsync(),
                    PageSize = request.PageSize,
                    PageNo = request.PageNo
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting expenses you owe", ex);
            }
        }

        public async Task<PagingResponse<Expense>> GetExpensesYouAreOwedAsync(int userId, PagingRequest request)
        {
            try
            {
                // Get expenses where user paid and others have shares
                var query = _context.Expenses
                    .Where(x => !x.IsDeleted &&
                        x.PaidByUserId == userId &&
                        x.ExpenseShares.Any(share => share.UserId != userId && !share.IsDeleted))
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .ThenInclude(x=>x.User)
                    .Include(x => x.PaidByUser)
                    .Include(x => x.Group)
                    .OrderByDescending(x => x.ExpenseDate)
                    .ThenByDescending(x => x.CreatedAt)
                    .AsQueryable();

                return new PagingResponse<Expense>
                {
                    Data = await query
                        .Skip((request.PageNo - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync(),
                    RecordCounts = await query.CountAsync(),
                    PageSize = request.PageSize,
                    PageNo = request.PageNo
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting expenses you are owed", ex);
            }
        }

        public async Task<List<MemberBalanceResponse>> GetMemberBalancesAsync(int userId)
        {
            try
            {
                // Get all expenses where user is involved
                var expenses = await _context.Expenses
                    .Where(x => !x.IsDeleted &&
                        (x.PaidByUserId == userId ||
                         x.ExpenseShares.Any(share => share.UserId == userId && !share.IsDeleted)))
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .ToListAsync();

                // Get all transactions involving this user
                var transactions = await _context.Transactions
                    .Where(t => !t.IsDeleted &&
                        (t.FromUserId == userId || t.ToUserId == userId))
                    .ToListAsync();

                // Get all unique user IDs involved
                var userIds = new HashSet<int>();
                foreach (var expense in expenses)
                {
                    if (expense.PaidByUserId != userId) userIds.Add(expense.PaidByUserId);
                    if (expense.ExpenseShares != null)
                    {
                        foreach (var share in expense.ExpenseShares.Where(s => s.UserId != userId))
                        {
                            userIds.Add(share.UserId);
                        }
                    }
                }

                // Fetch all users at once
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.UserId) && !u.IsDeleted)
                    .ToListAsync();

                var userDict = users.ToDictionary(u => u.UserId);

                // Dictionary to store balances per user
                var memberBalances = new Dictionary<int, MemberBalanceResponse>();

                // Calculate balances from expenses
                foreach (var expense in expenses)
                {
                    var paidByMe = expense.PaidByUserId == userId;

                    if (paidByMe)
                    {
                        // I paid - others owe me
                        if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                        {
                            foreach (var share in expense.ExpenseShares.Where(s => s.UserId != userId))
                            {
                                if (!memberBalances.ContainsKey(share.UserId))
                                {
                                    var user = userDict.GetValueOrDefault(share.UserId);
                                    memberBalances[share.UserId] = new MemberBalanceResponse
                                    {
                                        UserId = share.UserId,
                                        FirstName = user?.FirstName ?? "",
                                        LastName = user?.LastName ?? "",
                                        Email = user?.Email ?? "",
                                        ProfilePicture = user?.ThumbProfilePicture,
                                        YouOwe = 0,
                                        YouAreOwed = 0,
                                        NetBalance = 0
                                    };
                                }
                                memberBalances[share.UserId].YouAreOwed += share.AmountOwed;
                            }
                        }
                    }
                    else
                    {
                        // Someone else paid - check if I owe
                        if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                        {
                            var myShare = expense.ExpenseShares.FirstOrDefault(s => s.UserId == userId);
                            if (myShare != null)
                            {
                                var paidByUserId = expense.PaidByUserId;
                                if (!memberBalances.ContainsKey(paidByUserId))
                                {
                                    var user = userDict.GetValueOrDefault(paidByUserId);
                                    memberBalances[paidByUserId] = new MemberBalanceResponse
                                    {
                                        UserId = paidByUserId,
                                        FirstName = user?.FirstName ?? "",
                                        LastName = user?.LastName ?? "",
                                        Email = user?.Email ?? "",
                                        ProfilePicture = user?.ThumbProfilePicture,
                                        YouOwe = 0,
                                        YouAreOwed = 0,
                                        NetBalance = 0
                                    };
                                }
                                memberBalances[paidByUserId].YouOwe += myShare.AmountOwed;
                            }
                        }
                    }
                }

                // Adjust balances based on transactions
                foreach (var trans in transactions)
                {
                    if (trans.FromUserId == userId)
                    {
                        // I paid to someone - reduce what they owe me or increase what I owe them
                        if (memberBalances.ContainsKey(trans.ToUserId))
                        {
                            if (memberBalances[trans.ToUserId].YouAreOwed > 0)
                            {
                                // Reduce what they owe me
                                var reduction = Math.Min(trans.Amount, memberBalances[trans.ToUserId].YouAreOwed);
                                memberBalances[trans.ToUserId].YouAreOwed -= reduction;
                            }
                            else
                            {
                                // Increase what I owe them
                                memberBalances[trans.ToUserId].YouOwe += trans.Amount;
                            }
                        }
                    }
                    else if (trans.ToUserId == userId)
                    {
                        // Someone paid to me - reduce what I owe them or increase what they owe me
                        if (memberBalances.ContainsKey(trans.FromUserId))
                        {
                            if (memberBalances[trans.FromUserId].YouOwe > 0)
                            {
                                // Reduce what I owe them
                                var reduction = Math.Min(trans.Amount, memberBalances[trans.FromUserId].YouOwe);
                                memberBalances[trans.FromUserId].YouOwe -= reduction;
                            }
                            else
                            {
                                // Increase what they owe me
                                memberBalances[trans.FromUserId].YouAreOwed += trans.Amount;
                            }
                        }
                    }
                }

                // Calculate net balance for each member
                foreach (var member in memberBalances.Values)
                {
                    member.NetBalance = member.YouAreOwed - member.YouOwe;
                }

                return memberBalances.Values
                    .Where(m => m.NetBalance != 0) // Only return members with non-zero balance
                    .OrderByDescending(m => m.NetBalance)
                    .ThenBy(m => m.FirstName)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting member balances", ex);
            }
        }

        public async Task<bool> SettleAmountAsync(int fromUserId, int toUserId, decimal amount, string? description)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                // Create a transaction record
                var settlement = new Transaction
                {
                    FromUserId = fromUserId,
                    ToUserId = toUserId,
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = fromUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Transactions.Add(settlement);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error settling amount", ex);
            }
        }

        public async Task<List<MonthlyExpenseSummaryResponse>> GetMonthlyExpenseSummaryAsync(int userId, int months)
        {
            try
            {
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddMonths(-months);

                // Get all expenses where user is involved
                var expenses = await _context.Expenses
                    .Where(x => !x.IsDeleted &&
                        x.ExpenseDate >= startDate &&
                        x.ExpenseDate <= endDate &&
                        (x.PaidByUserId == userId ||
                         x.ExpenseShares.Any(share => share.UserId == userId && !share.IsDeleted)))
                    .Include(x => x.ExpenseShares.Where(share => !share.IsDeleted))
                    .ToListAsync();

                // Group by month
                var monthlyData = expenses
                    .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Expenses = g.ToList()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToList();

                var result = new List<MonthlyExpenseSummaryResponse>();

                foreach (var monthData in monthlyData)
                {
                    decimal totalAmount = 0;
                    decimal youOwe = 0;
                    decimal youAreOwed = 0;
                    int expenseCount = 0;

                    foreach (var expense in monthData.Expenses)
                    {
                        totalAmount += expense.Amount;
                        expenseCount++;

                        var paidByMe = expense.PaidByUserId == userId;

                        if (paidByMe)
                        {
                            // I paid - others owe me
                            if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                            {
                                var totalOwedByOthers = expense.ExpenseShares
                                    .Where(share => share.UserId != userId)
                                    .Sum(share => share.AmountOwed);
                                youAreOwed += totalOwedByOthers;
                            }
                        }
                        else
                        {
                            // Someone else paid - check if I owe
                            if (expense.ExpenseShares != null && expense.ExpenseShares.Any())
                            {
                                var myShare = expense.ExpenseShares.FirstOrDefault(share => share.UserId == userId);
                                if (myShare != null)
                                {
                                    youOwe += myShare.AmountOwed;
                                }
                            }
                        }
                    }

                    var monthName = new DateTime(monthData.Year, monthData.Month, 1).ToString("MMM yyyy");

                    result.Add(new MonthlyExpenseSummaryResponse
                    {
                        Month = monthName,
                        Year = monthData.Year,
                        MonthNumber = monthData.Month,
                        TotalAmount = totalAmount,
                        ExpenseCount = expenseCount,
                        YouOwe = youOwe,
                        YouAreOwed = youAreOwed,
                        NetBalance = youAreOwed - youOwe
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting monthly expense summary", ex);
            }
        }
    }
}

