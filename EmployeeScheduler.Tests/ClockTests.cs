using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeScheduler.Server.Clock;
using EmployeeScheduler.Server.Data;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;
using EmployeeScheduler.Server.ShiftManagement;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeScheduler.Tests
{
    public class ClockTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task ClockIn_CreatesEntry()
        {
            var db = CreateInMemoryDb();

            db.Users.Add(new User { Id = 1, Name = "User", Email = "u@test.com", PasswordHash = "x", Role = "employee" });
            db.Shifts.Add(new Shift { Id = 1, UserId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(8) });
            await db.SaveChangesAsync();

            var shiftAccessor = new ShiftAccessor(db);
            var shiftEngine = new ShiftEngine(shiftAccessor);

            var accessor = new ClockAccessor(db);
            var engine = new ClockEngine(accessor, shiftEngine);
            var manager = new ClockManager(engine);

            var result = await manager.ClockInAsync(1, 1);

            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(1, result.ShiftId);
            Assert.NotEqual(default, result.ClockInTime);
            Assert.Null(result.ClockOutTime);
        }

        [Fact]
        public async Task ClockOut_UpdatesEntry()
        {
            var db = CreateInMemoryDb();

            db.Users.Add(new User { Id = 2, Name = "User2", Email = "u2@test.com", PasswordHash = "x", Role = "employee" });
            db.Shifts.Add(new Shift { Id = 2, UserId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(8) });
            await db.SaveChangesAsync();

            var shiftAccessor = new ShiftAccessor(db);
            var shiftEngine = new ShiftEngine(shiftAccessor);

            var accessor = new ClockAccessor(db);
            var engine = new ClockEngine(accessor, shiftEngine);
            var manager = new ClockManager(engine);

            await manager.ClockInAsync(2, 2);
            var result = await manager.ClockOutAsync(2, 2);

            Assert.NotNull(result.ClockOutTime);
            Assert.True(result.ClockOutTime > result.ClockInTime);
        }

        [Fact]
        public async Task Prevents_DoubleClockIn()
        {
            var db = CreateInMemoryDb();

            db.Users.Add(new User { Id = 3, Name = "User3", Email = "u3@test.com", PasswordHash = "x", Role = "employee" });
            db.Shifts.Add(new Shift { Id = 3, UserId = 3, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(8) });
            await db.SaveChangesAsync();

            var shiftAccessor = new ShiftAccessor(db);
            var shiftEngine = new ShiftEngine(shiftAccessor);

            var accessor = new ClockAccessor(db);
            var engine = new ClockEngine(accessor, shiftEngine);
            var manager = new ClockManager(engine);

            await manager.ClockInAsync(3, 3);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                manager.ClockInAsync(3, 3));
        }

        [Fact]
        public async Task Prevents_ClockOut_Without_ClockIn()
        {
            var db = CreateInMemoryDb();

            db.Users.Add(new User { Id = 4, Name = "User4", Email = "u4@test.com", PasswordHash = "x", Role = "employee" });
            db.Shifts.Add(new Shift { Id = 4, UserId = 4, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(8) });
            await db.SaveChangesAsync();

            var shiftAccessor = new ShiftAccessor(db);
            var shiftEngine = new ShiftEngine(shiftAccessor);

            var accessor = new ClockAccessor(db);
            var engine = new ClockEngine(accessor, shiftEngine);
            var manager = new ClockManager(engine);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                manager.ClockOutAsync(4, 4));
        }

        [Fact]
        public async Task GetClockEntriesForUser_ReturnsEntries()
        {
            var db = CreateInMemoryDb();

            db.Users.Add(new User { Id = 5, Name = "User5", Email = "u5@test.com", PasswordHash = "x", Role = "employee" });
            db.Shifts.Add(new Shift { Id = 5, UserId = 5, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(8) });
            await db.SaveChangesAsync();

            var shiftAccessor = new ShiftAccessor(db);
            var shiftEngine = new ShiftEngine(shiftAccessor);

            var accessor = new ClockAccessor(db);
            var engine = new ClockEngine(accessor, shiftEngine);
            var manager = new ClockManager(engine);

            await manager.ClockInAsync(5, 5);
            var entries = await manager.GetClockEntriesForUserAsync(5);

            Assert.Single(entries);
            Assert.Equal(5, entries.First().UserId);
        }

        [Fact]
        public async Task GetClockEntriesForShift_ReturnsEntries()
        {
            var db = CreateInMemoryDb();

            db.Users.Add(new User { Id = 6, Name = "User6", Email = "u6@test.com", PasswordHash = "x", Role = "employee" });
            db.Shifts.Add(new Shift { Id = 6, UserId = 6, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(8) });
            await db.SaveChangesAsync();

            var shiftAccessor = new ShiftAccessor(db);
            var shiftEngine = new ShiftEngine(shiftAccessor);

            var accessor = new ClockAccessor(db);
            var engine = new ClockEngine(accessor, shiftEngine);
            var manager = new ClockManager(engine);

            await manager.ClockInAsync(6, 6);
            var entries = await manager.GetClockEntriesForShiftAsync(6);

            Assert.Single(entries);
            Assert.Equal(6, entries.First().ShiftId);
        }
    }
}
