using Microsoft.EntityFrameworkCore;
using Requalify.Connection;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Model;
using Requalify.Services;
using Xunit;

namespace Requalify.Tests.Services
{
    public class EducationServiceTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
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
        public async Task CreateAsync_ShouldCreateEducation_WhenValid()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            var request = new CreateEducationRequest
            {
                Degree = "Computer Science",
                Instituion = "USP",
                CompletionDate = DateTime.UtcNow.AddYears(-1),
                Certificate = "https://certificado.pdf",
                UserId = 1
            };

            var result = await service.CreateAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Computer Science", result.Degree);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDegreeMissing()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            var request = new CreateEducationRequest
            {
                Degree = "",
                Instituion = "USP",
                CompletionDate = DateTime.UtcNow,
                Certificate = "",
                UserId = 1
            };

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenInstitutionMissing()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            var request = new CreateEducationRequest
            {
                Degree = "CS",
                Instituion = "",
                CompletionDate = DateTime.UtcNow,
                UserId = 1
            };

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenUserDoesNotExist()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);

            var request = new CreateEducationRequest
            {
                Degree = "CS",
                Instituion = "USP",
                CompletionDate = DateTime.UtcNow,
                UserId = 99
            };

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEducation_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            context.Educations.Add(new Education
            {
                Id = 10,
                Degree = "ADS",
                Instituion = "Fatec",
                CompletionDate = DateTime.UtcNow,
                UserId = 1
            });

            context.SaveChanges();

            var result = await service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("ADS", result.Degree);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEducations_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            context.Educations.Add(new Education
            {
                Id = 1,
                Degree = "Engenharia",
                Instituion = "USP",
                CompletionDate = DateTime.UtcNow,
                UserId = 1
            });

            context.SaveChanges();

            var result = await service.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrow_WhenEmpty()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.GetAllAsync());
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnEducations_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            context.Educations.Add(new Education
            {
                Id = 1,
                Degree = "Medicina",
                Instituion = "USP",
                CompletionDate = DateTime.UtcNow,
                UserId = 1
            });

            context.SaveChanges();

            var result = await service.GetByUserIdAsync(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldThrow_WhenUserHasNoEducation()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.GetByUserIdAsync(1));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEducation_WhenValid()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            context.Educations.Add(new Education
            {
                Id = 2,
                Degree = "Administração",
                Instituion = "PUC",
                CompletionDate = DateTime.UtcNow.AddYears(-3),
                UserId = 1
            });

            context.SaveChanges();

            var request = new UpdateEducationRequest
            {
                Degree = "Administração Atualizada",
                Instituion = "PUC-SP",
                CompletionDate = DateTime.UtcNow
            };

            var updated = await service.UpdateAsync(2, request);

            Assert.Equal("Administração Atualizada", updated.Degree);
            Assert.Equal("PUC-SP", updated.Instituion);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);

            var request = new UpdateEducationRequest
            {
                Degree = "Something",
                Instituion = "Somewhere",
                CompletionDate = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.UpdateAsync(999, request));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEducation_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);
            SeedUser(context, 1);

            context.Educations.Add(new Education
            {
                Id = 3,
                Degree = "Direito",
                Instituion = "Mackenzie",
                CompletionDate = DateTime.UtcNow,
                UserId = 1
            });

            context.SaveChanges();

            await service.DeleteAsync(3);

            Assert.False(context.Educations.Any(e => e.Id == 3));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new EducationService(context);

            await Assert.ThrowsAsync<EducationNotFoundException>(() => service.DeleteAsync(12345));
        }
    }
}
