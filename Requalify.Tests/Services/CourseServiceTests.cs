using Microsoft.EntityFrameworkCore;
using Requalify.Connection;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Model;
using Requalify.Services;
using Xunit;

namespace Requalify.Tests.Services
{
    public class CourseServiceTests
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
                Nome = "User Test",
                Email = "user@test.com",
                Senha = "123456",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1),
                CargoAtual = "Developer",
                AreaInteresse = "Backend"
            });

            context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCourse_WhenValid()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            var request = new CreateCourseRequest
            {
                Title = "C# Basics",
                Description = "Intro course",
                Category = "Programming",
                Difficulty = "Easy",
                Url = "https://course.com",
                UserId = 1
            };

            var result = await service.CreateAsync(request);

            Assert.NotNull(result);
            Assert.Equal("C# Basics", result.Title);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTitleMissing()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            var request = new CreateCourseRequest
            {
                Title = "",
                Description = "AAA",
                Category = "BBB",
                Difficulty = "CCC",
                UserId = 1
            };

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenUserNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);

            var request = new CreateCourseRequest
            {
                Title = "Java",
                Description = "AAA",
                Category = "BBB",
                Difficulty = "Medium",
                UserId = 999
            };

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCourse_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            context.Courses.Add(new Course
            {
                Id = 10,
                Title = "SQL",
                Description = "Database course",
                Category = "Data",
                Difficulty = "Medium",
                Url = "url",
                UserId = 1
            });

            context.SaveChanges();

            var result = await service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("SQL", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.GetByIdAsync(456));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            context.Courses.Add(new Course
            {
                Id = 1,
                Title = "Python",
                Description = "Learn Python",
                Category = "Programming",
                Difficulty = "Easy",
                Url = "url",
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
            var service = new CourseService(context);

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.GetAllAsync());
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnCourses_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            context.Courses.Add(new Course
            {
                Id = 1,
                Title = "Node.js",
                Description = "JS backend",
                Category = "Programming",
                Difficulty = "Medium",
                Url = "url",
                UserId = 1
            });

            context.SaveChanges();

            var result = await service.GetByUserIdAsync(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldThrow_WhenNone()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.GetByUserIdAsync(1));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCourse_WhenValid()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            context.Courses.Add(new Course
            {
                Id = 88,
                Title = "Old Title",
                Description = "Old Desc",
                Category = "Old Cat",
                Difficulty = "Hard",
                Url = "old",
                UserId = 1
            });

            context.SaveChanges();

            var request = new UpdateCourseRequest
            {
                Title = "New Title",
                Description = "New Desc",
                Category = "New Cat",
                Difficulty = "Medium",
                Url = "new"
            };

            var updated = await service.UpdateAsync(88, request);

            Assert.Equal("New Title", updated.Title);
            Assert.Equal("New Cat", updated.Category);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);

            var request = new UpdateCourseRequest
            {
                Title = "X",
                Description = "Y",
                Category = "Z",
                Difficulty = "Medium",
                Url = ""
            };

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.UpdateAsync(999, request));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteCourse_WhenExists()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);
            SeedUser(context, 1);

            context.Courses.Add(new Course
            {
                Id = 5,
                Title = "Rust Course",
                Description = "Memory safety",
                Category = "Programming",
                Difficulty = "Hard",
                Url = "url",
                UserId = 1
            });

            context.SaveChanges();

            await service.DeleteAsync(5);

            Assert.False(context.Courses.Any(c => c.Id == 5));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotFound()
        {
            var context = CreateInMemoryDb();
            var service = new CourseService(context);

            await Assert.ThrowsAsync<CourseNotFoundException>(() => service.DeleteAsync(444));
        }
    }
}
