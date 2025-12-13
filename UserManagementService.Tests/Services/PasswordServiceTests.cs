using FluentAssertions;
using UserManagementService.Services;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UserManagementService.Tests.Services
{
    public class PasswordServiceTests
    {
        [Fact]
        public void HashAndVerify_ShouldReturnTrue_ForCorrectPassword()
        {
            var service = new PasswordService();
            var password = "SecurePassword123!";

            var hash = service.HashPassword(password);
            var result = service.VerifyPassword(password, hash);

            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_ShouldReturnFalse_ForIncorrectPassword()
        {
            var service = new PasswordService();
            var hash = service.HashPassword("CorrectPassword");

            var result = service.VerifyPassword("WrongPassword", hash);

            result.Should().BeFalse();
        }
    }
}
