using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace UserManagementService.IntegrationTests
{
    public class UsersControllerTests(CustomWebApplicationFactory factory)
                : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        [Fact]
        public async Task GetUsers_ReturnsSeededUsers()
        {
            var response = await Client.GetAsync("/api/users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("testuser");
        }
    }
}
