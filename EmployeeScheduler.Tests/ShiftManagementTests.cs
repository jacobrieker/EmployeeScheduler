using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using EmployeeScheduler.Server.Data;
using EmployeeScheduler.Server.Models;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.ShiftManagement;

namespace EmployeeScheduler.Tests
{
    public class ShiftManagementTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ShiftTestDb_" + Guid.NewGuid())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateShiftAsync_AddsShiftToDatabase()
        {
            var db = CreateInMemoryDb();
            var accessor = new ShiftAccessor(db);
            var engine = new ShiftEngine(accessor);
            var manager = new ShiftManager(engine);

            var user = new User { Name = "Shifter", Email = "shift@test.com", PasswordHash = "pw", Role = "employee" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var request = new CreateShiftRequest
            {
                UserId = user.Id,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(8)
            };

            var result = await manager.CreateShiftAsync(request);

            var shiftInDb = await db.Shifts.FirstOrDefaultAsync();
            Assert.NotNull(shiftInDb);
            Assert.Equal(user.Id, shiftInDb.UserId);
            Assert.Equal(request.StartTime, shiftInDb.StartTime);
            Assert.Equal(request.EndTime, shiftInDb.EndTime);
        }

        [Fact]
        public async Task GetShiftsForUserAsync_ReturnsShifts()
        {
            var db = CreateInMemoryDb();
            var accessor = new ShiftAccessor(db);
            var engine = new ShiftEngine(accessor);
            var manager = new ShiftManager(engine);

            var user = new User { Name = "Worker", Email = "worker@test.com", PasswordHash = "pw", Role = "employee" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            db.Shifts.AddRange(
                new Shift { UserId = user.Id, StartTime = DateTime.Today, EndTime = DateTime.Today.AddHours(8) },
                new Shift { UserId = user.Id, StartTime = DateTime.Today.AddDays(1), EndTime = DateTime.Today.AddDays(1).AddHours(8) }
            );
            await db.SaveChangesAsync();

            var results = await manager.GetShiftsForUserAsync(user.Id);

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetShiftsForUserInRangeAsync_ReturnsOnlyShiftsInRange()
        {
            var db = CreateInMemoryDb();
            var accessor = new ShiftAccessor(db);
            var engine = new ShiftEngine(accessor);
            var manager = new ShiftManager(engine);

            var user = new User { Name = "RangeGuy", Email = "range@test.com", PasswordHash = "pw", Role = "employee" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            db.Shifts.AddRange(
                new Shift { UserId = user.Id, StartTime = DateTime.Today.AddDays(-1), EndTime = DateTime.Today },
                new Shift { UserId = user.Id, StartTime = DateTime.Today.AddDays(2), EndTime = DateTime.Today.AddDays(2).AddHours(8) }
            );
            await db.SaveChangesAsync();

            var results = await manager.GetShiftsForUserInRangeAsync(user.Id, DateTime.Today, DateTime.Today.AddDays(3));

            Assert.Single(results);
            Assert.Equal(DateTime.Today.AddDays(2), results[0].StartTime);
        }

        [Fact]
        public async Task UpdateShiftAsync_UpdatesShiftCorrectly()
        {
            var db = CreateInMemoryDb();
            var accessor = new ShiftAccessor(db);
            var engine = new ShiftEngine(accessor);
            var manager = new ShiftManager(engine);

            var shift = new Shift { UserId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(4) };
            db.Shifts.Add(shift);
            await db.SaveChangesAsync();

            var updateRequest = new UpdateShiftRequest
            {
                ShiftId = shift.Id,
                StartTime = shift.StartTime.AddHours(1),
                EndTime = shift.EndTime.AddHours(1)
            };

            var updated = await manager.UpdateShiftAsync(updateRequest);

            Assert.NotNull(updated);
            Assert.Equal(updateRequest.StartTime, updated!.StartTime);
            Assert.Equal(updateRequest.EndTime, updated.EndTime);
        }

        [Fact]
        public async Task DeleteShiftAsync_RemovesShiftFromDatabase()
        {
            var db = CreateInMemoryDb();
            var accessor = new ShiftAccessor(db);
            var engine = new ShiftEngine(accessor);
            var manager = new ShiftManager(engine);

            var shift = new Shift { UserId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(2) };
            db.Shifts.Add(shift);
            await db.SaveChangesAsync();

            var deleted = await manager.DeleteShiftAsync(shift.Id);

            Assert.True(deleted);
            Assert.Null(await db.Shifts.FindAsync(shift.Id));
        }
    }
}
