﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
    
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });
                      
            modelBuilder.Entity<CourseStudying>(b =>
            {
                b.HasMany(e => e.Courses)
                    .WithOne()
                    .HasForeignKey(fk => fk.Id)
                    .IsRequired();
                b.HasMany(e => e.Students)
                    .WithOne()
                    .HasForeignKey(fk => fk.Id)
                    .IsRequired();
            });

            modelBuilder.Entity<Quiz>(b =>
            {
                b.ToTable(e => e.HasCheckConstraint("PassThreshold", "PassThreshold > 0 AND PassThreshold < 1"));
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
            });

            modelBuilder.Entity<CourseLection>(b =>
            {
                b.HasMany(e=> e.Quizes).WithOne().OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}