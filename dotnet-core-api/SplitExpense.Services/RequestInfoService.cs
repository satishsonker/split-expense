using Microsoft.AspNetCore.Http;

namespace SplitExpense.Services
{
    public class RequestInfoService(IHttpContextAccessor httpContextAccessor) : IRequestInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public string GetClientIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return "Unknown";
                }
                
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ipAddress = forwardedFor.Split(',')[0].Trim();
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        return ipAddress;
                    }
                }
                
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp.Trim();
                }
                
                var remoteIp = httpContext.Connection.RemoteIpAddress;
                if (remoteIp != null)
                {
                    // Handle IPv6 mapped to IPv4
                    if (remoteIp.IsIPv4MappedToIPv6)
                    {
                        return remoteIp.MapToIPv4().ToString();
                    }
                    return remoteIp.ToString();
                }

                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        public string GetDeviceInfo()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return "Unknown";
                }

                var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
                if (string.IsNullOrEmpty(userAgent))
                {
                    return "Unknown";
                }
                
                var deviceInfo = ParseUserAgent(userAgent);
                return deviceInfo;
            }
            catch
            {
                return "Unknown";
            }
        }
        
        private string ParseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return "Unknown";
            }

            var parts = new List<string>();
            
            if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Mobile");
            }
            
            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
            {
                if (userAgent.Contains("Windows NT 10.0", StringComparison.OrdinalIgnoreCase))
                    parts.Add("Windows 10/11");
                else if (userAgent.Contains("Windows NT 6.3", StringComparison.OrdinalIgnoreCase))
                    parts.Add("Windows 8.1");
                else if (userAgent.Contains("Windows NT 6.2", StringComparison.OrdinalIgnoreCase))
                    parts.Add("Windows 8");
                else if (userAgent.Contains("Windows NT 6.1", StringComparison.OrdinalIgnoreCase))
                    parts.Add("Windows 7");
                else
                    parts.Add("Windows");
            }
            else if (userAgent.Contains("Mac OS X", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("macOS");
            }
            else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Linux");
            }
            else if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Android");
            }
            else if (userAgent.Contains("iOS", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("iOS");
            }
            
            if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase) && !userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Chrome");
            }
            else if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Firefox");
            }
            else if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase) && !userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Safari");
            }
            else if (userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Edge");
            }
            else if (userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("OPR", StringComparison.OrdinalIgnoreCase))
            {
                parts.Add("Opera");
            }
            
            if (parts.Count == 0)
            {                
                if (userAgent.Length > 100)
                {
                    return string.Concat(userAgent.AsSpan(0, 100), "...");
                }
                return userAgent;
            }

            return string.Join(" / ", parts);
        }
    }
}
