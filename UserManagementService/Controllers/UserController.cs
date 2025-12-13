using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserManagementService.DTOs.Users;
using UserManagementService.Models;
using UserManagementService.Services;

namespace UserManagementService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        // CREATE USER
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = new User
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                Language = request.Language,
                Culture = request.Culture
            };

            var createdUser = await _userService.CreateUserAsync(user, request.Password);

            return CreatedAtAction(nameof(GetUserById),
                new { id = createdUser.Id },
                MapToResponse(createdUser));
        }


        // GET ALL USERS
        // GET /api/users? page = 1 & pageSize = 20
        [HttpGet]
        public async Task<IActionResult> GetUsers(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var users = await _userService.GetUsersAsync(page, pageSize);
            var totalCount = await _userService.GetUserCountAsync();

            var response = new
            {
                page,
                pageSize,
                totalCount,
                data = users.Select(MapToResponse)
            };

            return Ok(response);
        }

        // GET USER BY ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(MapToResponse(user));
        }

        // UPDATE USER
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            await _userService.UpdateUserAsync(id, user =>
            {
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.MobileNumber = request.MobileNumber;
                user.Language = request.Language;
                user.Culture = request.Culture;
            });

            return NoContent();
        }

        // DELETE USER
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        // VALIDATE PASSWORD
        [HttpPost("{id:guid}/validate-password")]
        [EnableRateLimiting("passwordLimiter")]
        public async Task<IActionResult> ValidatePassword(Guid id, [FromBody] ValidatePasswordRequest request)
        {
            var isValid = await _userService.ValidatePasswordAsync(id, request.Password);
            return Ok(new { isValid });
        }

        private static UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Language = user.Language,
                Culture = user.Culture
            };
        }
    }
}
