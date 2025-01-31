﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SplitExpense.Data.DbModels;
using SplitExpense.EmailManagement.Models;

namespace SplitExpense.Data
{
    public class SplitExpenseDbContext(DbContextOptions<SplitExpenseDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroupMapping> UserGroupMappings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<SmtpSettings> SmtpSettings { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }



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

                if (entityEntry.Entity is not BaseModels.BaseDbModels baseModel) continue;

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