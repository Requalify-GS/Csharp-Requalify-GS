using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Requalify.Model;

namespace Requalify.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("TB_USERS", "RM554694");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .Metadata.SetColumnName("ID");

            builder.Property(u => u.Nome)
                   .IsRequired()
                   .HasMaxLength(100)
                   .Metadata.SetColumnName("NOME");

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(200)
                   .Metadata.SetColumnName("EMAIL");

            builder.Property(u => u.Senha)
                   .IsRequired()
                   .HasMaxLength(200)
                   .Metadata.SetColumnName("SENHA");

            builder.Property(u => u.Telefone)
                   .HasMaxLength(20)
                   .Metadata.SetColumnName("TELEFONE");

            builder.Property(u => u.DataNascimento)
                   .Metadata.SetColumnName("DATA_NASCIMENTO");

            builder.Property(u => u.CargoAtual)
                   .HasMaxLength(150)
                   .Metadata.SetColumnName("CARGO_ATUAL");

            builder.Property(u => u.AreaInteresse)
                   .HasMaxLength(150)
                   .Metadata.SetColumnName("AREA_INTERESSE");

            // Relacionamentos
            builder.HasMany(u => u.Educations)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Skills)
                   .WithOne(s => s.User)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Courses)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
