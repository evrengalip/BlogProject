using BlogProject.Entity.DTOs.Users;
using BlogProject.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Superadmin,Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersWithRoleAsync();
            return Ok(users);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetAppUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var role = await _userService.GetUserRoleAsync(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Role = role
            };

            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserAddDto userAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUserAsync(userAddDto);

            if (result.Succeeded)
                return StatusCode(201, new { message = "User created successfully" });

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(userUpdateDto);

            if (result.Succeeded)
                return Ok(new { message = "User updated successfully" });

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (result.identityResult.Succeeded)
                return Ok(new { message = $"User '{result.email}' deleted successfully" });

            return BadRequest(new { errors = result.identityResult.Errors.Select(e => e.Description) });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _userService.GetUserProfileAsync();
            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfileDto userProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UserProfileUpdateAsync(userProfileDto);

            if (result)
                return Ok(new { message = "Profile updated successfully" });

            return BadRequest(new { message = "Failed to update profile" });
        }
    }
}