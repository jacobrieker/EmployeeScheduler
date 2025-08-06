using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeScheduler.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeScheduler.Server.ShiftManagement
{
    public class ShiftEngine : IShiftEngine
    {
        private readonly IShiftAccessor _accessor;

        public ShiftEngine(IShiftAccessor accessor)
        {
            _accessor = accessor;
        }

        public async Task<Shift> CreateShiftAsync(int userId, DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ArgumentException("End time must be after start time.");

            var shift = new Shift
            {
                UserId = userId,
                StartTime = start,
                EndTime = end
            };

            return await _accessor.CreateShiftAsync(shift);
        }

        public async Task<List<Shift>> GetShiftsForUserAsync(int userId)
        {
            return await _accessor.GetShiftsForUserAsync(userId);
        }

        public async Task<List<Shift>> GetShiftsForUserInRangeAsync(int userId, DateTime from, DateTime to)
        {
            return await _accessor.GetShiftsForUserInRangeAsync(userId, from, to);
        }

        public async Task<Shift?> GetShiftByIdAsync(int shiftId)
        {
            return await _accessor.GetShiftByIdAsync(shiftId);
        }

        public async Task<Shift?> UpdateShiftAsync(Shift shift)
        {
            return await _accessor.UpdateShiftAsync(shift);
        }

        public async Task<bool> DeleteShiftAsync(int shiftId)
        {
            return await _accessor.DeleteShiftAsync(shiftId);
        }

        public async Task<List<Shift>> GetAllShiftsAsync()
        {
            return await _accessor.GetAllShiftsAsync();
        }

    }
}
