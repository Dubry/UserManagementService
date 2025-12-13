using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using UserManagementService.DTOs.Users;
using UserManagementService.Models;
using Xunit;

namespace UserManagementService.IntegrationTests
{
    public class ApiKeyTests : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly HttpClient _client;

        public ApiKeyTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            // Add API key header for all requests
            _client.DefaultRequestHeaders.Add("X-API-KEY", "test-api-key");
        }

        [Fact]
        public async Task Request_WithoutApiKey_Returns401()
        {
            var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/users");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Request_WithValidApiKey_Returns200()
        {
            // Act
            var response = await _client.GetAsync("/api/users");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            var users = await response.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            users.Should().NotBeNull();
            users?.Data.Should().Contain(u => u.UserName == "testuser");
            // TEMP DEBUG
            response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_With200()
        {
            // Arrange - get the seeded user's ID
            var usersResponse = await _client.GetAsync("/api/users?page=1&pageSize=10");
            var users = await usersResponse.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            var testUserId = users?.Data.First().Id;

            // Act
            var response = await _client.GetAsync($"/api/users/{testUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<User>();
            user.UserName.Should().Be("testuser");
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedUser_With200()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                UserName = "newuser",
                FullName = "New User",
                Email = "newuser@test.com",
                MobileNumber = "+1987654321",
                Language = "en",
                Culture = "en-US",
                Password = "Password123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            createdUser?.UserName.Should().Be("newuser");
        }

        [Fact]
        public async Task UpdateUser_ReturnsUpdatedUser_With200()
        {
            // Arrange
            var usersResponse = await _client.GetAsync("/api/users?page=1&pageSize=10");
            var users = await usersResponse.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            var userId = users?.Data.First().Id;

            var updateRequest = new UpdateUserRequest
            {
                FullName = "Updated Name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Fetch the user again to verify update
            var getResponse = await _client.GetAsync($"/api/users/{userId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedUser = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            updatedUser.FullName.Should().Be("Updated Name");
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent()
        {
            // Arrange
            var usersResponse = await _client.GetAsync("/api/users?page=1&pageSize=10");
            var users = await usersResponse.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            var userId = users?.Data.First().Id;

            // Act
            var response = await _client.DeleteAsync($"/api/users/{userId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/users/{userId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
