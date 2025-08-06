using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.UserAccount;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace EmployeeScheduler.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountManager _manager;

        public UserAccountController(IUserAccountManager manager)
        {
            _manager = manager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            try
            {
                var result = await _manager.RegisterAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _manager.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
                new Claim(ClaimTypes.Name, result.Name ?? ""),
                new Claim(ClaimTypes.Email, result.Email ?? ""),
                new Claim(ClaimTypes.Role, result.Role ?? "")
            };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _manager.GetUserByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var result = await _manager.UpdateUserAsync(id, request);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest request)
        {
            var result = await _manager.ChangePasswordAsync(id, request);
            if (result == null)
                return BadRequest(new { message = "Password change failed. Please check your current password and confirm your new password." });

            return Ok(result);
        }

        [HttpPost("{id}/uploadProfilePic")]
        public async Task<IActionResult> UploadProfilePic(int id)
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-pics");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/profile-pics/{uniqueFileName}";

            var user = await _manager.UpdateUserAsync(id, new UpdateUserRequest
            {
                Name = null,
                Email = null,
                ProfilePictureUrl = relativePath
            });

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _manager.GetAllUsersAsync();
            var responses = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                ProfilePictureUrl = u.ProfilePic
            }).ToList();

            return Ok(responses);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string newRole)
        {
            var user = await _manager.UpdateUserRoleAsync(id, newRole);
            if (user == null)
                return NotFound();

            return Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ProfilePictureUrl = user.ProfilePic
            });
        }

    }
}
