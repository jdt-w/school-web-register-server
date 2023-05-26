using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        CourseRepository(ApplicationDbContext db) : base(db) 
        {

        }

        public Task<Student> GetStudentsList(int courseId)
        {
            throw new NotImplementedException();
            //    var myOb = Context.Database.ExecuteSqlRaw(
            //@$"select  * 
            //  from    db1.dbo.table1 t1
            //  join    db2.dbo.table2 t2
            //  on      t2.t1_id = t1.id
            //  where   t1.id  = {table1Id}").FirstOrDefault();
        }
    }
}
