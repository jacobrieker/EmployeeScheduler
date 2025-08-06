using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using EmployeeScheduler.Server.UserAccount;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Data;
using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Tests
{
    public class UserAccountTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task RegisterUser_SavesUserToDatabase()
        {
            var dbContext = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(dbContext);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var request = new RegisterUserRequest
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "secret123"
            };

            var response = await manager.RegisterAsync(request);

            var savedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            Assert.NotNull(savedUser);
            Assert.Equal(request.Name, savedUser!.Name);
            Assert.Equal(request.Email, savedUser.Email);
            Assert.Equal("employee", savedUser.Role);
            Assert.NotEqual(request.Password, savedUser.PasswordHash);
        }

        [Fact]
        public async Task LoginUser_ReturnsCorrectUser()
        {
            var dbContext = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(dbContext);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var hashed = BCrypt.Net.BCrypt.HashPassword("secret123");
            dbContext.Users.Add(new User
            {
                Name = "Existing User",
                Email = "login@example.com",
                PasswordHash = hashed,
                Role = "employee"
            });
            await dbContext.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = "login@example.com",
                Password = "secret123"
            };

            var result = await manager.LoginAsync(loginRequest);

            Assert.NotNull(result);
            Assert.Equal("Existing User", result!.Name);
            Assert.Equal("login@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserById_ReturnsCorrectUser()
        {
            var dbContext = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(dbContext);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var user = new User
            {
                Name = "ById User",
                Email = "byid@example.com",
                PasswordHash = "dummy",
                Role = "employee"
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var result = await manager.GetUserByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("ById User", result!.Name);
            Assert.Equal("byid@example.com", result.Email);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsUser()
        {
            var db = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(db);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var password = "mypassword";
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Name = "User", Email = "test@login.com", PasswordHash = hashed, Role = "employee" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var request = new LoginRequest { Email = "test@login.com", Password = password };

            var result = await manager.LoginAsync(request);

            Assert.NotNull(result);
            Assert.Equal("User", result.Name);
            Assert.Equal("test@login.com", result.Email);
            Assert.Equal("employee", result.Role);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ReturnsNull()
        {
            var db = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(db);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var hashed = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            db.Users.Add(new User { Name = "User", Email = "wrong@pw.com", PasswordHash = hashed, Role = "employee" });
            await db.SaveChangesAsync();

            var request = new LoginRequest { Email = "wrong@pw.com", Password = "wrongpassword" };

            var result = await manager.LoginAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithExistingId_ReturnsUser()
        {
            var db = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(db);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var user = new User { Name = "Test User", Email = "byid@test.com", PasswordHash = "pw", Role = "employee" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var result = await manager.GetUserByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("Test User", result.Name);
            Assert.Equal("byid@test.com", result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithNonExistingId_ReturnsNull()
        {
            var db = CreateInMemoryDb();
            var accessor = new UserAccountAccessor(db);
            var engine = new UserAccountEngine(accessor);
            var manager = new UserAccountManager(engine);

            var result = await manager.GetUserByIdAsync(999);

            Assert.Null(result);
        }


    }
}
