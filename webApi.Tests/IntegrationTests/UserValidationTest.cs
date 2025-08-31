// webApi.Tests/IntegrationTests/SqlInjectionTests.cs
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using webApi.Data;
using webApi.Models;

namespace webApi.Tests.IntegrationTests
{
    [TestFixture]
    public class SqlInjectionTests
    {
        private SqliteConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        [TearDown]
        public void TearDown()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        [Test]
        public void AddUser_WithMaliciousInput_DoesNotExecuteSQLCommands()
        {
            // Configuración del DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(options);
            context.Database.EnsureCreated();

            // Test code aquí...
        }
    }
}