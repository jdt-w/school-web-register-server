using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Logging
{
    public enum EventType : ushort
    {
        UserAuthenticated = 0,
        UserLogout = 1,



        OpenPage = 1000,
        OpenFile = 1001,
        
    }

    public interface ILoggingService
    {
        void LogInformation(string message);
        void LogInformation(string message, params object[] args);
        void LogEventAction(ActionLog log);
    }
}
