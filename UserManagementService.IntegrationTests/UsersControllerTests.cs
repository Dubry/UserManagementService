using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using UserManagementService.DTOs.Users;
using UserManagementService.Models;
using Xunit;

namespace UserManagementService.IntegrationTests
{
    public class UsersControllerTests(CustomWebApplicationFactory factory)
                : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region GET Users

        [Fact]
        public async Task GetUsers_ReturnsSeededUsers()
        {
            var response = await Client.GetAsync("/api/users");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("testuser");
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_With200()
        {
            // Arrange - get the seeded user's ID
            var usersResponse = await Client.GetAsync("/api/users?page=1&pageSize=10");
            var users = await usersResponse.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            var testUserId = users?.Data.First().Id;

            // Act
            var response = await Client.GetAsync($"/api/users/{testUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<User>();
            user.UserName.Should().Be("testuser");
        }

        #endregion

        #region Create Users

        [Fact]
        public async Task CreateUser_ReturnsCreatedUser_With200()
        {
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

            var response = await Client.PostAsJsonAsync("/api/users", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            createdUser?.UserName.Should().Be("newuser");
        }

        [Fact]
        public async Task CreateUser_MissingRequiredFields_ReturnsBadRequest()
        {
            var invalidUser = new CreateUserRequest { UserName = "", Password = "" };
            var response = await Client.PostAsJsonAsync("/api/users", invalidUser);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateUser_DuplicateUsername_ReturnsConflict()
        {
            var existingUser = new CreateUserRequest
            {
                UserName = "testuser",
                Password = "Password123!",
                Email = "original@test.com"
            };

            await Client.PostAsJsonAsync("/api/users", existingUser);

            var duplicateUser = new CreateUserRequest
            {
                UserName = "testuser", 
                Password = "Password123!",
                Email = "duplicate@test.com"
            };

            var response = await Client.PostAsJsonAsync("/api/users", duplicateUser);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("{\"StatusCode\":409,\"Error\":\"UserAlreadyExistsException\",");
        }

        #endregion

        #region UPDATE Users

        [Fact]
        public async Task UpdateUser_ReturnsUpdatedUser_With200()
        {
            // Arrange
            var usersResponse = await Client.GetAsync("/api/users?page=1&pageSize=10");
            var users = await usersResponse.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            var userId = users?.Data.First().Id;

            var updateRequest = new UpdateUserRequest
            {
                FullName = "Updated Name"
            };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Fetch the user again to verify update
            var getResponse = await Client.GetAsync($"/api/users/{userId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedUser = await getResponse.Content.ReadFromJsonAsync<UserResponse>();
            updatedUser.FullName.Should().Be("Updated Name");
        }

        [Fact]
        public async Task UpdateUser_InvalidId_ReturnsNotFound()
        {
            var updateRequest = new UpdateUserRequest { FullName = "NoOne" };
            var response = await Client.PutAsJsonAsync($"/api/users/{Guid.NewGuid()}", updateRequest);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region DELETE Users

        [Fact]
        public async Task DeleteUser_ReturnsNoContent()
        {
            // Seed a user specifically for deletion
            var createUser = new CreateUserRequest
            {
                UserName = "deleteuser",
                Password = "Password123!",
                Email = "deleteuser@test.com"
            };

            var createResponse = await Client.PostAsJsonAsync("/api/users", createUser);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUser = await createResponse.Content.ReadFromJsonAsync<UserResponse>();
            createdUser.Should().NotBeNull();

            var deleteResponse = await Client.DeleteAsync($"/api/users/{createdUser!.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"/api/users/{createdUser.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteUser_InvalidId_ReturnsNotFound()
        {
            var response = await Client.DeleteAsync($"/api/users/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Pagination Tests

        [Fact]
        public async Task GetUsers_Pagination_Works()
        {
            var response = await Client.GetAsync("/api/users?page=1&pageSize=1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var users = await response.Content.ReadFromJsonAsync<PagedResponse<UserResponse>>();
            users.Should().NotBeNull();
            users?.Page.Should().Be(1);
            users?.PageSize.Should().Be(1);
        }

        #endregion
    }
}
