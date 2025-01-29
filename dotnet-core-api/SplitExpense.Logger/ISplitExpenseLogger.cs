using System.Runtime.CompilerServices;

namespace SplitExpense.Logger
{
    public interface ISplitExpenseLogger
    {
        void LogError(string message, string errorCode = null, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null);
        void LogError(Exception ex);
        void LogError(Exception ex,string message);
        void LogError(Exception ex, string message,string errorCode);
        void LogWarning(string message, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null);
        void LogInfo(string message, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null);
        void LogDebug(Exception ex,string message,  [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null);
        void LogDebug(string message, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null);
    }
}