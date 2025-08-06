using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.ShiftManagement
{
    public interface IShiftEngine
    {
        Task<Shift> CreateShiftAsync(int userId, DateTime start, DateTime end);
        Task<List<Shift>> GetShiftsForUserAsync(int userId);
        Task<List<Shift>> GetShiftsForUserInRangeAsync(int userId, DateTime from, DateTime to);
        Task<Shift?> GetShiftByIdAsync(int shiftId);
        Task<Shift?> UpdateShiftAsync(Shift shift);
        Task<bool> DeleteShiftAsync(int shiftId);
        Task<List<Shift>> GetAllShiftsAsync();
    }
}
