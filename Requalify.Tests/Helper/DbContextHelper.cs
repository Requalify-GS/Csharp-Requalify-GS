using Microsoft.EntityFrameworkCore;
using Requalify.Connection;

namespace Requalify.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
