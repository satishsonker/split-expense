namespace SplitExpense.Logging
{
    public enum ErrorCodes
    { // General Errors
        GeneralError = 1000,
        UnknownError = 1001,
        InvalidRequest = 1002,
        Unauthorized = 1003,
        Forbidden = 1004,
        NotFound = 1005,
        Conflict = 1006,
        TooManyRequests = 1007,

        // Validation Errors
        ValidationFailed = 2000,
        MissingRequiredField = 2001,
        InvalidFieldFormat = 2002,

        // Database Errors
        DatabaseConnectionFailed = 3000,
        RecordNotFound = 3001,
        RecordConflict = 3002,
        QueryExecutionError = 3003,

        // External Service Errors
        ExternalServiceUnavailable = 4000,
        ExternalServiceTimeout = 4001,
        ExternalServiceError = 4002,

        // Authentication and Authorization Errors
        AuthenticationFailed = 5000,
        TokenExpired = 5001,
        InsufficientPermissions = 5002,

        // Application-Specific Errors
        BusinessRuleViolation = 6000,
        DataProcessingError = 6001,
        ResourceLimitExceeded = 6002
    }
}
