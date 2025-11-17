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
            builder.ToTable("COURSE", "RM554694");

            // Chave primária
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .Metadata.SetColumnName("ID");

            // Title
            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(c => c.Title)
                   .Metadata.SetColumnName("TITLE");

            // Description
            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(c => c.Description)
                   .Metadata.SetColumnName("DESCRIPTION");

            // Category
            builder.Property(c => c.Category)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(c => c.Category)
                   .Metadata.SetColumnName("CATEGORY");

            // Difficulty
            builder.Property(c => c.Difficulty)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(c => c.Difficulty)
                   .Metadata.SetColumnName("DIFFICULTY");

            // Url
            builder.Property(c => c.Url)
                   .HasMaxLength(200); // opcional se URL pode ficar vazia
            builder.Property(c => c.Url)
                   .Metadata.SetColumnName("URL");

            // UserId (FK)
            builder.Property(c => c.UserId)
                   .IsRequired();
            builder.Property(c => c.UserId)
                   .Metadata.SetColumnName("USER_ID");

            // Relacionamento 1:N com User
            builder.HasOne(c => c.User)
                   .WithMany(u => u.Courses)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
