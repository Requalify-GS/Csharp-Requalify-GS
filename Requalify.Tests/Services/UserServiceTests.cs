using System.Threading.Tasks;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Services;
using Requalify.Tests.Helpers;
using Xunit;

namespace Requalify.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_Create_User_When_Data_Is_Valid()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var service = new UserService(context);

            var request = new CreateUserRequest
            {
                Nome = "Ana Silva",
                Email = "ana@example.com",
                Senha = "SenhaForte123!",
                Telefone = "11999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                CargoAtual = "Junior Developer",
                AreaInteresse = "Backend Development"
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotEqual(0, result.Id);
            Assert.Equal("Ana Silva", result.Nome);
            Assert.Single(context.Users);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Email_Is_Empty()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var service = new UserService(context);

            var request = new CreateUserRequest
            {
                Nome = "Ana Silva",
                Email = "",
                Senha = "SenhaForte123!",
                Telefone = "11999999999",
                DataNascimento = new DateTime(2000, 1, 1),
                CargoAtual = "Junior Developer",
                AreaInteresse = "Backend Development"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task GetAllAsync_Should_Throw_When_No_Users()
        {
            // Arrange
            var context = DbContextHelper.CreateInMemoryContext();
            var service = new UserService(context);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => service.GetAllAsync());
        }
    }
}
