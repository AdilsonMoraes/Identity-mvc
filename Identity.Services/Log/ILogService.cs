using Identity.Domain.Enums;

namespace Identity.Services.Log
{
    public interface ILogService
    {
        void LogMessage(string message, Guid sessionGuid, LogType logType, string applicationName = "IdentityWebApi");

        void LogException(Exception ex, Guid sessionGuid, string applicationName = "IdentityWebApi");
    }
}
