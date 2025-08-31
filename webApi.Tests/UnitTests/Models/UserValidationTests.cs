// webApi.Tests/UnitTests/Models/UserValidationTests.cs
using NUnit.Framework;
using webApi.Models;

namespace webApi.Tests.UnitTests.Models
{
    [TestFixture]
    public class UserValidationTests
    {
        [Test]
        public void Sanitize_WithXSSInput_RemovesMaliciousContent()
        {
            // Arrange
            var user = new User
            {
                Username = "<img src=x onerror=alert('xss')>",
                Email = "victim@example.com"
            };

            // Act
            user.Sanitize();

            // Assert
            Assert.That(user.Username, Does.Not.Contain("<img"));
            Assert.That(user.Username, Does.Not.Contain("onerror"));
            Assert.That(user.Username, Does.Not.Contain("alert('xss')"));
        }
    }
}