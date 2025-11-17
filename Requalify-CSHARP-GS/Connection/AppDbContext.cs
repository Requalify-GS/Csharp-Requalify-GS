using Microsoft.EntityFrameworkCore;
using Requalify.Data.Mappings;
using Requalify.Model;

namespace Requalify.Connection
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

            public DbSet<User> Users { get; set; }
            public DbSet<Skill> Skills { get; set; }
            public DbSet<Course> Courses { get; set; }
            public DbSet<Education> Educations { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new UserMapping());
                modelBuilder.ApplyConfiguration(new CourseMapping());
                modelBuilder.ApplyConfiguration(new EducationMapping());
                modelBuilder.ApplyConfiguration(new SkillMapping());
            }
    }
}

