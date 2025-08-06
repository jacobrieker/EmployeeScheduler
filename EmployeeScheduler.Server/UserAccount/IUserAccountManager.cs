using System.Threading.Tasks;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = EmployeeScheduler.Server.DTOs.LoginRequest;

namespace EmployeeScheduler.Server.UserAccount
{
    public interface IUserAccountManager
    {
        Task<UserResponse> RegisterAsync(RegisterUserRequest request);
        Task<UserResponse?> LoginAsync(LoginRequest request);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest updateRequest);
        Task<UserResponse?> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> UpdateUserRoleAsync(int id, string newRole);


    }
}
