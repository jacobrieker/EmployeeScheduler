using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.UserAccount;

public interface IUserAccountEngine
{
    Task<User> RegisterAsync(string name, string email, string password);
    Task<User?> LoginAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> UpdateUserAsync(int id, string name, string email, string? profilePictureUrl);
    Task<User?> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> UpdateUserRoleAsync(int id, string newRole);

}
