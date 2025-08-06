using System.Threading.Tasks;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = EmployeeScheduler.Server.DTOs.LoginRequest;

namespace EmployeeScheduler.Server.UserAccount
{
    public class UserAccountManager : IUserAccountManager
    {
        private readonly IUserAccountEngine _engine;

        public UserAccountManager(IUserAccountEngine engine)
        {
            _engine = engine;
        }

        public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
        {
            var user = await _engine.RegisterAsync(request.Name, request.Email, request.Password);
            return ToResponse(user);
        }

        public async Task<UserResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _engine.LoginAsync(request.Email, request.Password);
            return user == null ? null : ToResponse(user);
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await _engine.GetUserByIdAsync(id);
            return user == null ? null : ToResponse(user);
        }

        public async Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest updateRequest)
        {
            var currentUser = await _engine.GetUserByIdAsync(id);
            if (currentUser == null) return null;

            var updatedUser = await _engine.UpdateUserAsync(
                id,
                updateRequest.Name ?? currentUser.Name,
                updateRequest.Email ?? currentUser.Email,
                updateRequest.ProfilePictureUrl ?? currentUser.ProfilePic
            );

            return updatedUser == null ? null : ToResponse(updatedUser);
        }

        public async Task<UserResponse?> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
                return null;

            var updatedUser = await _engine.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            return updatedUser == null ? null : ToResponse(updatedUser);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _engine.GetAllUsersAsync();
        }

        public async Task<User?> UpdateUserRoleAsync(int id, string newRole)
        {
            return await _engine.UpdateUserRoleAsync(id, newRole);
        }


        private UserResponse ToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ProfilePictureUrl = user.ProfilePic
            };
        }
    }
}
