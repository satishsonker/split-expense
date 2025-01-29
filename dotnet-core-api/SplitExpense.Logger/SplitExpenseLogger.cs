using Microsoft.AspNetCore.Mvc;
using NLog;
using SplitExpense.Logging;
using System.Runtime.CompilerServices;

namespace SplitExpense.Logger
{
    public class SplitExpenseLogger: ISplitExpenseLogger
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private string FormatControllerName(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return "UnknownController";
            var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            return fileName ?? "UnknownController";
        }
        public void LogError(string message, string errorCode = null, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null)
        {
            _logger.Error($"Controller: {FormatControllerName(controllerName)}, Action: {actionName}, ErrorCode: {errorCode}, Message: {message}");
        }
        public void LogWarning(string message, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null)
        {
            _logger.Warn($"Controller: {FormatControllerName(controllerName)}, Action: {actionName}, Message: {message}");
        }
        public void LogInfo(string message, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null)
        {
            _logger.Info($"Controller: {FormatControllerName(controllerName)}, Action: {actionName}, Message: {message}");
        }
        public void LogDebug(string message, [CallerMemberName] string actionName = null, [CallerFilePath] string controllerName = null)
        {
            _logger.Debug($"Controller: {FormatControllerName(controllerName)}, Action: {actionName}, Message: {message}");
        }
        public void LogDebug(Exception ex, string message, string actionName = null, string controllerName = null)
        {
            _logger.Debug($"Controller: {FormatControllerName(controllerName)}, Action: {actionName}, Message: {message}, Exception: {ex}");
        }

        public void LogError(Exception ex)
        {
            _logger.Error($"{ex}");
        }

        public void LogError(Exception ex, string message)
        {
            _logger.Error($"Message: {message},ExceptionL {ex}");
        }

        public void LogError(Exception ex, string message,string errorCode)
        {
            _logger.Error($"Message: {message}, ErrorCode: {errorCode},ExceptionL {ex}");
        }
    }
}
