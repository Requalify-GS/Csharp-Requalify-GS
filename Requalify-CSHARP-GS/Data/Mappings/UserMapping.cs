using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Requalify.Model;

namespace Requalify.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Nome da tabela (TABLE, SCHEMA)
            builder.ToTable("USER", "RM554694");

            // Chave primária
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .Metadata.SetColumnName("ID");

            // Nome
            builder.Property(u => u.Nome)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(u => u.Nome)
                   .Metadata.SetColumnName("NOME");

            // Email
            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(u => u.Email)
                   .Metadata.SetColumnName("EMAIL");

            // Senha
            builder.Property(u => u.Senha)
                   .IsRequired()
                   .HasMaxLength(200); 
            builder.Property(u => u.Senha)
                   .Metadata.SetColumnName("SENHA");

            // Relacionamento 1:N – User -> Educations
            builder.HasMany(u => u.Educations)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N – User -> Skills
            builder.HasMany(u => u.Skills)
                   .WithOne(s => s.User)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N – User -> Courses
            builder.HasMany(u => u.Courses)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
