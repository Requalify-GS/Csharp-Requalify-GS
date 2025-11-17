using Requalify.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Requalify.Data.Mappings
{
    public class EducationMapping : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {
            // Nome da tabela e schema
            builder.ToTable("EDUCATION", "RM554694");

            // Chave primária
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .Metadata.SetColumnName("ID");

            builder.Property(e => e.Degree)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(e => e.Degree)
                   .Metadata.SetColumnName("DEGREE");

            builder.Property(e => e.Instituion)
                  .IsRequired();
            builder.Property(e => e.Instituion)
                  .Metadata.SetColumnName("INSTITUTION");

            builder.Property(e => e.CompletionDate)
                   .IsRequired();
            builder.Property(e => e.CompletionDate)
                   .Metadata.SetColumnName("COMPLETION_DATE");

            builder.Property(e => e.Certificate)
                   .IsRequired(); // depende da sua regra
            builder.Property(e => e.Certificate)
                   .Metadata.SetColumnName("CERTIFICATE");

            builder.Property(e => e.UserId)
                   .IsRequired();
            builder.Property(e => e.UserId)
                   .Metadata.SetColumnName("USER_ID");

            // Relacionamento 1:N → Education pertence a User
            builder.HasOne(e => e.User)
                   .WithMany(u => u.Educations)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
