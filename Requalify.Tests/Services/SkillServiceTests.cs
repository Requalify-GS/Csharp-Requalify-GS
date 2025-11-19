using Microsoft.EntityFrameworkCore;
using Requalify.Connection;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Model;
using Requalify.Services;
using Xunit;

namespace Requalify.Tests.Services
{
    public class SkillServiceTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private void SeedUser(AppDbContext context, int id)
        {
            context.Users.Add(new User
            {
                Id = id,
                Nome = "Test User",
                Email = "test@test.com",
                Senha = "123456",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1),
                CargoAtual = "Developer",
                AreaInteresse = "Backend"
            });

            context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSkill_WhenValid()
        {
            // Arrange
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            var request = new CreateSkillRequest
            {
                Name = "C#",
                Level = "Avançado",
                Category = "Backend",
                Description = "Habilidade em C#",
                ProficiencyPercentage = 90,
                UserId = 1
            };


            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C#", result.Name);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenNameIsMissing()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            var request = new CreateSkillRequest
            {
                Name = "",
                Level = "Advanced",
                Category = "Tech",
                ProficiencyPercentage = 50,
                UserId = 1
            };

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenUserDoesNotExist()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);

            var request = new CreateSkillRequest
            {
                Name = "C#",
                Level = "Advanced",
                Category = "Programming",
                ProficiencyPercentage = 60,
                UserId = 99
            };

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenProficiencyInvalid()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            var request = new CreateSkillRequest
            {
                Name = "Java",
                Level = "Basic",
                Category = "Programming",
                ProficiencyPercentage = 500, // INVALIDO
                UserId = 1
            };

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSkill_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            context.Skills.Add(new Skill
            {
                Id = 10,
                Name = "SQL",
                Level = "Intermediate",
                Category = "Database",
                ProficiencyPercentage = 70,
                UserId = 1
            });

            context.SaveChanges();

            var result = await service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("SQL", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnSkills_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            context.Skills.Add(new Skill
            {
                Id = 1,
                Name = "Python",
                Level = "Advanced",
                Category = "Programming",
                UserId = 1,
                ProficiencyPercentage = 80
            });

            context.SaveChanges();

            var result = await service.GetByUserIdAsync(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldThrow_WhenUserHasNoSkills()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.GetByUserIdAsync(1));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSkill_WhenValid()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            context.Skills.Add(new Skill
            {
                Id = 5,
                Name = "Java",
                Level = "Basic",
                Category = "Programming",
                ProficiencyPercentage = 10,
                UserId = 1
            });

            context.SaveChanges();

            var request = new UpdateSkillRequest
            {
                Name = "Java Updated",
                Level = "Intermediate",
                Category = "Programming",
                ProficiencyPercentage = 60
            };

            var result = await service.UpdateAsync(5, request);

            Assert.Equal("Java Updated", result.Name);
            Assert.Equal(60, result.ProficiencyPercentage);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);

            var request = new UpdateSkillRequest
            {
                Name = "Test",
                Level = "Basic",
                Category = "Tech",
                ProficiencyPercentage = 10
            };

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.UpdateAsync(999, request));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteSkill_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);
            SeedUser(context, 1);

            context.Skills.Add(new Skill
            {
                Id = 3,
                Name = "C++",
                Level = "Advanced",
                Category = "Programming",
                ProficiencyPercentage = 95,
                UserId = 1
            });

            context.SaveChanges();

            await service.DeleteAsync(3);

            Assert.False(context.Skills.Any(s => s.Id == 3));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new SkillService(context);

            await Assert.ThrowsAsync<SkillNotFoundException>(() => service.DeleteAsync(987));
        }
    }
}
