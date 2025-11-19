using Requalify.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Requalify.Data.Mappings
{
    public class CourseMapping : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Nome da tabela e schema
            builder.ToTable("TB_COURSE", "RM554694");

            // Chave primária
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .Metadata.SetColumnName("ID");

            // Title
            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(100)
                   .Metadata.SetColumnName("TITLE");

            // Description
            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(500)
                   .Metadata.SetColumnName("DESCRIPTION");

            // Category
            builder.Property(c => c.Category)
                   .IsRequired()
                   .HasMaxLength(50)
                   .Metadata.SetColumnName("CATEGORY");

            // Difficulty
            builder.Property(c => c.Difficulty)
                   .IsRequired()
                   .HasMaxLength(50)
                   .Metadata.SetColumnName("DIFFICULTY");

            // Url
            builder.Property(c => c.Url)
                   .HasMaxLength(200)
                   .Metadata.SetColumnName("URL");

            // CreatedAt
            builder.Property(c => c.CreatedAt)
                   .IsRequired()
                   .Metadata.SetColumnName("CREATED_AT");

            // UpdatedAt
            builder.Property(c => c.UpdatedAt)
                   .Metadata.SetColumnName("UPDATED_AT");

            // UserId (FK)
            builder.Property(c => c.UserId)
                   .IsRequired()
                   .Metadata.SetColumnName("USER_ID");

            // Relacionamento 1:N com User
            builder.HasOne(c => c.User)
                   .WithMany(u => u.Courses)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
