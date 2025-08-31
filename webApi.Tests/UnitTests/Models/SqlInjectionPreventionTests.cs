// webApi.Tests/UnitTests/Models/SqlInjectionPreventionTests.cs
using NUnit.Framework;
using webApi.Models;

namespace webApi.Tests.UnitTests.Models
{
    [TestFixture]
    public class SqlInjectionPreventionTests
    {
        [Test]
        public void Sanitize_WithSQLInjectionInUsername_EncodesOrRemovesDangerousCharacters()
        {
            // Arrange
            var user = new User
            {
                Username = "admin'; DROP TABLE Users; --",
                Email = "test@example.com"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Username, Does.Not.Contain("DROP TABLE"), 
                    "DROP TABLE command should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain(";"), 
                    "Semicolons should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain("--"), 
                    "SQL comments should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain("'"), 
                    "Single quotes should be encoded");
            });
        }

        [Test]
        public void Sanitize_WithSQLInjectionInEmail_EncodesOrRemovesDangerousCharacters()
        {
            // Arrange
            var user = new User
            {
                Username = "normalUser",
                Email = "test@example.com'; UPDATE Users SET Password = 'hacked' WHERE 1=1; --"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Email, Does.Not.Contain("UPDATE"), 
                    "UPDATE command should be removed or encoded");
                Assert.That(user.Email, Does.Not.Contain("WHERE 1=1"), 
                    "SQL conditions should be removed or encoded");
                Assert.That(user.Email, Does.Not.Contain(";"), 
                    "Semicolons should be removed or encoded");
                Assert.That(user.Email, Does.Not.Contain("--"), 
                    "SQL comments should be removed or encoded");
            });
        }

        [Test]
        public void Sanitize_WithUnionBasedSQLInjection_EncodesOrRemovesDangerousContent()
        {
            // Arrange
            var user = new User
            {
                Username = "admin' UNION SELECT password FROM Users WHERE '1'='1",
                Email = "test@example.com"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Username, Does.Not.Contain("UNION"), 
                    "UNION operator should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain("SELECT"), 
                    "SELECT command should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain("WHERE '1'='1"), 
                    "SQL conditions should be removed or encoded");
            });
        }

        [Test]
        public void Sanitize_WithTimeBasedSQLInjection_EncodesOrRemovesDangerousContent()
        {
            // Arrange
            var user = new User
            {
                Username = "admin'; WAITFOR DELAY '00:00:10'; --",
                Email = "test@example.com"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Username, Does.Not.Contain("WAITFOR"), 
                    "WAITFOR command should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain("DELAY"), 
                    "DELAY command should be removed or encoded");
                Assert.That(user.Username, Does.Not.Contain(";"), 
                    "Semicolons should be removed or encoded");
            });
        }

        [Test]
        public void Sanitize_WithMultipleSQLInjectionAttempts_HandlesAllCases()
        {
            // Arrange
            var user = new User
            {
                Username = "admin'; DROP TABLE Users; EXEC xp_cmdshell('format C:'); --",
                Email = "test@example.com' OR '1'='1"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Username, Does.Not.Contain("DROP TABLE"), 
                    "DROP TABLE should be removed");
                Assert.That(user.Username, Does.Not.Contain("EXEC"), 
                    "EXEC command should be removed");
                Assert.That(user.Username, Does.Not.Contain("xp_cmdshell"), 
                    "Dangerous procedures should be removed");
                Assert.That(user.Email, Does.Not.Contain("OR '1'='1"), 
                    "SQL conditions should be removed");
            });
        }

        [Test]
        public void Sanitize_WithSafeInput_DoesNotModifyContent()
        {
            // Arrange
            var safeUsername = "john_doe123";
            var safeEmail = "john.doe@example.com";
            var user = new User
            {
                Username = safeUsername,
                Email = safeEmail
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Username, Is.EqualTo(safeUsername), 
                    "Safe username should not be modified");
                Assert.That(user.Email, Is.EqualTo(safeEmail), 
                    "Safe email should not be modified");
            });
        }

        [Test]
        public void Sanitize_WithSpecialButSafeCharacters_PreservesThem()
        {
            // Arrange
            var user = new User
            {
                Username = "user.name-with_dots",
                Email = "user.name+filter@example.com"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(user.Username, Does.Contain("."), 
                    "Dots should be preserved in usernames");
                Assert.That(user.Username, Does.Contain("-"), 
                    "Dashes should be preserved in usernames");
                Assert.That(user.Username, Does.Contain("_"), 
                    "Underscores should be preserved in usernames");
                Assert.That(user.Email, Does.Contain("."), 
                    "Dots should be preserved in emails");
                Assert.That(user.Email, Does.Contain("+"), 
                    "Plus signs should be preserved in emails");
                Assert.That(user.Email, Does.Contain("@"), 
                    "At signs should be preserved in emails");
            });
        }
    }
}