using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface ILogRepository
    {
        Task LogActionEvent(ActionLog log);
    }
}
