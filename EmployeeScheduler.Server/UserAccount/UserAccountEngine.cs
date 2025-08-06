using System.Threading.Tasks;
using EmployeeScheduler.Server.Models;
using BCrypt.Net;


namespace EmployeeScheduler.Server.UserAccount
{
    public class UserAccountEngine : IUserAccountEngine
    {
        private readonly IUserAccountAccessor _accessor;

        public UserAccountEngine(IUserAccountAccessor accessor)
        {
            _accessor = accessor;
        }

        public async Task<User> RegisterAsync(string name, string email, string password)
        {
            var existing = await _accessor.GetUserByEmailAsync(email);
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            var hash = HashPassword(password);

            var isFirstUser = !(await _accessor.GetAllUsersAsync()).Any();

            var role = isFirstUser ? "owner" : "employee";

            return await _accessor.InsertUserAsync(name, email, hash, role, null);
        }


        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _accessor.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _accessor.GetUserByIdAsync(id);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }
        public async Task<User?> UpdateUserAsync(int id, string name, string email, string? profilePictureUrl)
        {
            return await _accessor.UpdateUserAsync(id, name, email, profilePictureUrl);
        }

        public async Task<User?> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _accessor.GetUserByIdAsync(userId);
            if (user == null) return null;

            if (!VerifyPassword(currentPassword, user.PasswordHash))
                return null;

            var newHashedPassword = HashPassword(newPassword);
            return await _accessor.ChangePasswordAsync(userId, newHashedPassword);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _accessor.GetAllUsersAsync();
        }

        public async Task<User?> UpdateUserRoleAsync(int id, string newRole)
        {
            return await _accessor.UpdateUserRoleAsync(id, newRole);
        }



    }
}
