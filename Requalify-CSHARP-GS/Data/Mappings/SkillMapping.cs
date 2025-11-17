using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Requalify.Model;

namespace Requalify.Data.Mappings
{
    public class SkillMapping : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            // Nome da tabela (schema + table)
            builder.ToTable("SKILL", "RM554694");

            // Chave primária
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .Metadata.SetColumnName("ID");

            // Name
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(s => s.Name)
                   .Metadata.SetColumnName("NAME");

            // Level
            builder.Property(s => s.Level)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(s => s.Level)
                   .Metadata.SetColumnName("LEVEL");

            // Category
            builder.Property(s => s.Category)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(s => s.Category)
                   .Metadata.SetColumnName("CATEGORY");

            // ProficiencyPercentage
            builder.Property(s => s.ProficiencyPercentage)
                   .IsRequired();
            builder.Property(s => s.ProficiencyPercentage)
                   .Metadata.SetColumnName("PROFICIENCY_PERCENTAGE");

            // Description
            builder.Property(s => s.Description)
                   .HasMaxLength(500);
            builder.Property(s => s.Description)
                   .Metadata.SetColumnName("DESCRIPTION");

            // FK UserId
            builder.Property(s => s.UserId)
                   .IsRequired();
            builder.Property(s => s.UserId)
                   .Metadata.SetColumnName("USER_ID");

            // Relacionamento 1:N Skill → User
            builder.HasOne(s => s.User)
                   .WithMany(u => u.Skills)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
