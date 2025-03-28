﻿using SplitExpense.SharedResource;

namespace SplitExpense.ExceptionManagement.Exceptions
{
    public class BusinessRuleViolationException : Exception
    {
        public BusinessRuleViolationException(ErrorCodes errorCode)
        {
            ErrorResponseType = errorCode.ToString();
            Errors = errorCode.GetDescription();
        }

        public BusinessRuleViolationException(string errorResponseType, string message, object? errors = null) :
            base(message)
        {
            ErrorResponseType = errorResponseType;
            Errors = errors;
        }

        public BusinessRuleViolationException(int errorCode, string message, object? errors = null) :
           base(message)
        {
            ErrorResponseType = errorCode.ToString();
            Errors = errors;
        }

        public string ErrorResponseType { get; }

        public object? Errors { get; }
    }
}