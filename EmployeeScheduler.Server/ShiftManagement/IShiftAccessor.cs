using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.ShiftManagement
{
    public interface IShiftAccessor
    {
        Task<Shift> CreateShiftAsync(Shift shift);
        Task<List<Shift>> GetShiftsForUserAsync(int userId);
        Task<List<Shift>> GetShiftsForUserInRangeAsync(int userId, DateTime from, DateTime to);
        Task<Shift?> GetShiftByIdAsync(int shiftId);
        Task<Shift?> UpdateShiftAsync(Shift shift);
        Task<bool> DeleteShiftAsync(int shiftId);
        Task<List<Shift>> GetAllShiftsAsync();
    }
}
