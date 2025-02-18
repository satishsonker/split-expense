using System.ComponentModel;

namespace SplitExpense.SharedResource
{
    public enum ErrorCodes
    {
        [Description("General error")]
        GeneralError = 1000,
        [Description("Unknown error")]
        UnknownError = 1001,
        [Description("Invalid request")]
        InvalidRequest = 1002,
        [Description("Unauthorized")]
        Unauthorized = 1003,
        [Description("Forbidden")]
        Forbidden = 1004,
        [Description("Not found")]
        NotFound = 1005,
        [Description("conflict")]
        Conflict = 1006,
        [Description("Too many requests")]
        TooManyRequests = 1007,

        // Validation Errors
        [Description("Validation failed")]
        ValidationFailed = 2000,
        [Description("Missing required field")]
        MissingRequiredField = 2001,
        [Description("Invalid field format")]
        InvalidFieldFormat = 2002, 
        [Description("Invalid File/Image")]
        InvalidFile = 2003,
        [Description("Invalid File/Image Extension")]
        InvalidFileExtension = 2004,
        [Description("File/Image SizeExceeded")]
        FileSizeExceeded = 2005,

        // Database Errors
        [Description("Database connection failed")]
        DatabaseConnectionFailed = 3000,
        [Description("Record not found")]
        RecordNotFound = 3001,
        [Description("Record conflict")]
        RecordConflict = 3002,
        [Description("Query execution error")]
        QueryExecutionError = 3003,
        [Description("Unable to add record in database")]
        UnableToAddRecord = 3004,
        [Description("Record already exist")]
        RecordAlreadyExist = 3005,
        [Description("Unable to update record in database")]
        UnableToUpdateRecord = 3006,


        // External Service Errors
        [Description("External service unavailable")]
        ExternalServiceUnavailable = 4000,
        [Description("External service timeout")]
        ExternalServiceTimeout = 4001,
        [Description("External service error")]
        ExternalServiceError = 4002,

        // Authentication and Authorization Errors
        [Description("Authentication failed")]
        AuthenticationFailed = 5000,
        [Description("Token expired")]
        TokenExpired = 5001,
        [Description("Insufficient permissions")]
        InsufficientPermissions = 5002,

        // Application-Specific Errors
        [Description("Business rule violation")]
        BusinessRuleViolation = 6000,
        [Description("Data processing error")]
        DataProcessingError = 6001,
        [Description("Resource limit exceeded")]
        ResourceLimitExceeded = 6002,
        InvalidData = 6003
    }
}
