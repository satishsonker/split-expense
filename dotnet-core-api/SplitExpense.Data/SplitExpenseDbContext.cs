using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Models;
using SplitExpense.Models.DbModels;

namespace SplitExpense.Data
{
    public class SplitExpenseDbContext(DbContextOptions<SplitExpenseDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public DbSet<ExpenseGroup> Groups { get; set; }
        public DbSet<UserGroupMapping> UserGroupMappings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<SplitType> SplitTypes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseReceipt> ExpenseReceipts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ExpenseNote> ExpenseNotes { get; set; }
        public DbSet<ExpenseShare> ExpenseShares { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<GroupDetail> GroupDetails { get; set; }
        public DbSet<ExpenseActivity> ExpenseActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            AddDateTimeStamp();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddDateTimeStamp();
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private void AddDateTimeStamp()
        {
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                var hasChange = entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Modified;
                if (!hasChange) continue;

                if (entityEntry.Entity is not BaseDbModels baseModel) continue;

                var now = DateTime.Now;
                int userId = 0;
                if ((bool)_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("userId"))
                {
                    string value = _httpContextAccessor.HttpContext?.Request.Headers["userId"].ToString();
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new UnauthorizedAccessException("UserId not Found");
                    }
                    if (!int.TryParse(value, out userId))
                    {
                        throw new UnauthorizedAccessException("Invalid userId");
                    }

                }
                if (entityEntry.State is EntityState.Added)
                {
                    baseModel.CreatedBy = userId;
                    baseModel.CreatedAt = now;
                    var userIdProperty = entityEntry.Entity.GetType().GetProperty("UserId");
                    if (userIdProperty != null && userIdProperty.PropertyType == typeof(int))
                    {
                        userIdProperty.SetValue(entityEntry.Entity, userId);
                    }
                }
                else
                {
                    baseModel.UpdatedBy = userId;
                    entityEntry.Property("CreatedAt").IsModified = false;
                    baseModel.UpdatedAt = now;
                }
            }
        }
    }
}