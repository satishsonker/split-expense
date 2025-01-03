﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Data.DbModels;

namespace SplitExpense.Data
{
    public class SplitExpenseDbContext(DbContextOptions<SplitExpenseDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}