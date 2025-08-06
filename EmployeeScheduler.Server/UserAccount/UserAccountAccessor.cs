using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeScheduler.Server.Models;
using UserModel = EmployeeScheduler.Server.Models.User;
using EmployeeScheduler.Server.Data;

namespace EmployeeScheduler.Server.UserAccount
{
    public class UserAccountAccessor : IUserAccountAccessor
    {
        private readonly AppDbContext _db;

        public UserAccountAccessor(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserModel?> GetUserByIdAsync(int id)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserModel> InsertUserAsync(string name, string email, string passwordHash, string role, string? profilePictureUrl = null)
        {
            var user = new UserModel
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                ProfilePic = profilePictureUrl
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel?> UpdateUserAsync(int id, string name, string email, string? profilePictureUrl)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            user.Name = name;
            user.Email = email;
            user.ProfilePic = profilePictureUrl;

            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<UserModel?> ChangePasswordAsync(int userId, string newHashedPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            user.PasswordHash = newHashedPassword;
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User?> UpdateUserRoleAsync(int id, string newRole)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            user.Role = newRole;
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> ids)
        {
            return await _db.Users
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }


    }
}