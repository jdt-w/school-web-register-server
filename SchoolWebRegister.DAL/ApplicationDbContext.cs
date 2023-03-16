using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(e => e.Claims)
                   .WithOne(e => e.User)
                   .HasForeignKey(uc => uc.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Profile)
                    .WithOne(e => e.User)
                    .HasForeignKey<Profile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Quiz>(b =>
            {
                b.ToTable(e => e.HasCheckConstraint("PassThreshold", "PassThreshold > 0 AND PassThreshold < 1"));
            });

            modelBuilder.Entity<QuizQuestion>(b =>
            {
                b.HasOne(e => e.Quiz)
                    .WithMany(e => e.Questions)
                    .HasForeignKey(fk => fk.QuizId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuizAnswer>(b =>
            {
                b.HasOne(e => e.Question)
                    .WithMany(e => e.Answers)
                    .HasForeignKey(fk => fk.QuestionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Course>(b =>
            {
                b.HasOne(e => e.Author).WithOne();

                b.HasMany(c => c.Students)
                    .WithMany(s => s.Courses)
                    .UsingEntity<CourseEnrollment>(j => j
                        .HasOne(pt => pt.Student)
                        .WithMany(t => t.Enrollments)
                        .HasForeignKey(pt => pt.StudentId)
                        .OnDelete(DeleteBehavior.Restrict), j => j

                        .HasOne(pt => pt.Course)
                        .WithMany(p => p.Enrollments)
                        .HasForeignKey(pt => pt.CourseId)
                        .OnDelete(DeleteBehavior.Cascade), j =>
                        {
                            j.Property(pt => pt.Progress).HasDefaultValue(0);
                            j.HasKey(t => new { t.CourseId, t.StudentId });
                            j.ToTable("CourseEnrollments");
                        });
                });

            modelBuilder.Entity<Student>(b =>
            {
                b.ToTable("Students");
            });

            modelBuilder.Entity<CourseLection>(b =>
            {
                b.HasMany(e => e.Quizes).WithOne().OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
