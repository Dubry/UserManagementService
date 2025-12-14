using FluentAssertions;
using Serilog;
using System.Net;
using System.Net.Http.Json;
using UserManagementService.Data;
using UserManagementService.DTOs.Users;
using UserManagementService.Models;
using Xunit;

namespace UserManagementService.IntegrationTests
{
    public class ApiKeyTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region ApiKey tests

        [Fact]
        public async Task Request_WithoutApiKey_Returns401()
        {
            Client.DefaultRequestHeaders.Remove("X-API-KEY");

            var response = await Client.GetAsync("/api/users");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RequestWithInvalidApiKey_Returns401()
        {
            Client.DefaultRequestHeaders.Add("X-API-KEY", "invalid-key");

            var response = await Client.GetAsync(Client.BaseAddress + "api/users");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RequestWithInactiveApiKey_Returns401()
        {
            Client.DefaultRequestHeaders.Add("X-API-KEY", "inactive-key");

            var response = await Client.GetAsync(Client.BaseAddress + "api/users");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Request_WithValidApiKey_Returns200()
        {
            // Act
            var response = await Client.GetAsync("/api/users");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            var users = await response.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            users.Should().NotBeNull();
            users?.Data.Should().Contain(u => u.UserName == "testuser");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Rate Limiting Tests

        [Fact]
        public async Task ExceedRateLimit_Returns429()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                UserName = "ratelimituser",
                Password = "Password123!"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/users", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUser = await response.Content.ReadFromJsonAsync<User>();

            var url = $"/api/users/{createdUser.Id}/validate-password";
            var requestBody = new ValidatePasswordRequest
            {
                Password = "any-password"
            };

            HttpResponseMessage lastResponse = null!;
            for (int i = 0; i < 6; i++) 
            {
                lastResponse = await Client.PostAsJsonAsync(url, requestBody);
            }

            lastResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task RateLimit_Reset_AllowsNewRequests()
        {
            for (int i = 0; i < 5; i++)
            {
                var resp = await Client.GetAsync(Client.BaseAddress + "api/users");
                resp.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            await Task.Delay(TimeSpan.FromMinutes(1.1));

            // Next request should succeed
            var response = await Client.GetAsync(Client.BaseAddress + "api/users");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion
    }
}
