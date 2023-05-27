using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface ILogRepository : IDbRepository<ActionLog>
    {
        Task LogActionEvent(ActionLog log);
    }
}
