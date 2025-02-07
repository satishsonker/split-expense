﻿namespace SplitExpense.EmailManagement.Service
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
