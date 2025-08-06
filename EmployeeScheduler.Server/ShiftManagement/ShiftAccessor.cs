using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmployeeScheduler.Server.Data;
using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.ShiftManagement
{
    public class ShiftAccessor : IShiftAccessor
    {
        private readonly AppDbContext _db;

        public ShiftAccessor(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Shift> CreateShiftAsync(Shift shift)
        {
            _db.Shifts.Add(shift);
            await _db.SaveChangesAsync();
            return shift;
        }

        public async Task<List<Shift>> GetShiftsForUserAsync(int userId)
        {
            return await _db.Shifts
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Shift>> GetShiftsForUserInRangeAsync(int userId, DateTime from, DateTime to)
        {
            return await _db.Shifts
                .Where(s => s.UserId == userId && s.StartTime >= from && s.EndTime <= to)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Shift?> GetShiftByIdAsync(int shiftId)
        {
            return await _db.Shifts.FirstOrDefaultAsync(s => s.Id == shiftId);
        }

        public async Task<Shift?> UpdateShiftAsync(Shift shift)
        {
            var existing = await _db.Shifts.FindAsync(shift.Id);
            if (existing == null) return null;

            existing.StartTime = shift.StartTime;
            existing.EndTime = shift.EndTime;
            existing.UserId = shift.UserId;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteShiftAsync(int shiftId)
        {
            var shift = await _db.Shifts.FindAsync(shiftId);
            if (shift == null) return false;

            _db.Shifts.Remove(shift);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<Shift>> GetAllShiftsAsync()
        {
            return await _db.Shifts
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

    }
}
