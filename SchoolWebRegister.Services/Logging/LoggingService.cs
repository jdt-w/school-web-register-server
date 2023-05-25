using Microsoft.Extensions.Logging;
using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Logging
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly ILogRepository _logRepository;
        public LoggingService(ILogger<LoggingService> logger, ILogRepository repository)
        {
            _logger = logger;
            _logRepository = repository;
        }

        public void LogEventAction(ActionLog log)
        {
            _logRepository.LogActionEvent(log);
        }
        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }
    }
}
