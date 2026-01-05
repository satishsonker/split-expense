namespace SplitExpense.Services
{
    public interface IRequestInfoService
    {
        string GetClientIpAddress();
        
        string GetDeviceInfo();
    }
}
