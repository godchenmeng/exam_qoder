using Xunit;
using ExamSystem.Infrastructure.Utils;

namespace ExamSystem.Tests.Infrastructure
{
    /// <summary>
    /// 密码工具类测试
    /// </summary>
    public class PasswordHelperTests
    {
        [Fact]
        public void HashPassword_ShouldReturnNonEmptyString()
        {
            // Arrange
            var password = "test123";

            // Act
            var hash = PasswordHelper.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void HashPassword_SamPassword_ShouldReturnSameHash()
        {
            // Arrange
            var password = "test123";

            // Act
            var hash1 = PasswordHelper.HashPassword(password);
            var hash2 = PasswordHelper.HashPassword(password);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "test123";
            var hash = PasswordHelper.HashPassword(password);

            // Act
            var result = PasswordHelper.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "test123";
            var wrongPassword = "wrong123";
            var hash = PasswordHelper.HashPassword(password);

            // Act
            var result = PasswordHelper.VerifyPassword(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GenerateRandomPassword_ShouldReturnCorrectLength()
        {
            // Arrange
            var length = 8;

            // Act
            var password = PasswordHelper.GenerateRandomPassword(length);

            // Assert
            Assert.Equal(length, password.Length);
        }

        [Theory]
        [InlineData("123456", true)]
        [InlineData("abc123", true)]
        [InlineData("12345", false)]  // 太短
        [InlineData("", false)]  // 空
        public void ValidatePasswordStrength_ShouldValidateCorrectly(string password, bool expected)
        {
            // Act
            var result = PasswordHelper.ValidatePasswordStrength(password);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
