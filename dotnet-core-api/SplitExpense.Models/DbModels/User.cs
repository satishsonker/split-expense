﻿using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models.DbModels
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePicture { get; set; }
        public string? LanguageCode { get; set; } = "us-en";
        public string? CurrencyCode { get; set; } = "INR";
        public string? CountryCode { get; set; }
        public string? ISDCode { get; set; }
        public bool IsDeleted {  get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
