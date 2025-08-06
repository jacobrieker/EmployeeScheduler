using EmployeeScheduler.Server.Models;
namespace EmployeeScheduler.Server.UserAccount
{
    public interface IUserAccountAccessor
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> InsertUserAsync(string name, string email, string passwordHash, string role, string? profilePictureUrl = null);
        Task<User?> UpdateUserAsync(int id, string name, string email, string? profilePictureUrl);
        Task<User?> ChangePasswordAsync(int userId, string newHashedPassword);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> UpdateUserRoleAsync(int id, string newRole);
        Task<List<User>> GetUsersByIdsAsync(List<int> ids);

    }

}
