using EmployeeScheduler.Server.Data;
using EmployeeScheduler.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeScheduler.Server.Clock
{
    public class ClockAccessor : IClockAccessor
    {
        private readonly AppDbContext _db;

        public ClockAccessor(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ClockEntry> CreateClockEntryAsync(ClockEntry entry)
        {
            _db.ClockEntries.Add(entry);
            await _db.SaveChangesAsync();
            return entry;
        }

        public async Task<ClockEntry?> GetClockEntryByIdAsync(int id)
        {
            return await _db.ClockEntries.FindAsync(id);
        }

        public async Task<List<ClockEntry>> GetClockEntriesForUserAsync(int userId)
        {
            return await _db.ClockEntries
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<ClockEntry>> GetClockEntriesForShiftAsync(int shiftId)
        {
            return await _db.ClockEntries
                .Where(e => e.ShiftId == shiftId)
                .ToListAsync();
        }

        public async Task<ClockEntry?> UpdateClockEntryAsync(ClockEntry entry)
        {
            var existing = await _db.ClockEntries.FindAsync(entry.Id);
            if (existing == null) return null;

            existing.ClockIn = entry.ClockIn;
            existing.ClockOut = entry.ClockOut;
            existing.UserId = entry.UserId;
            existing.ShiftId = entry.ShiftId;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<List<ClockEntry>> GetAllClockEntriesAsync()
        {
            return await _db.ClockEntries.ToListAsync();
        }

        public async Task<bool> DeleteClockEntryAsync(int id)
        {
            var entry = await _db.ClockEntries.FindAsync(id);
            if (entry == null) return false;

            _db.ClockEntries.Remove(entry);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
