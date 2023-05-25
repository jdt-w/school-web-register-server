using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public class LogRepository : BaseRepository<ActionLog>, ILogRepository
    {
        public LogRepository(ApplicationDbContext db) : base(db)
        {

        }
        public async Task LogActionEvent(ActionLog log)
        {
            await base.AddAsync(log);
        }
    }
}
