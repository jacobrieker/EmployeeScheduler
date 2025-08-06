using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.Clock
{
    public interface IClockAccessor
    {
        Task<ClockEntry> CreateClockEntryAsync(ClockEntry entry);
        Task<ClockEntry?> GetClockEntryByIdAsync(int id);
        Task<List<ClockEntry>> GetClockEntriesForUserAsync(int userId);
        Task<List<ClockEntry>> GetClockEntriesForShiftAsync(int shiftId);
        Task<ClockEntry?> UpdateClockEntryAsync(ClockEntry entry);
        Task<bool> DeleteClockEntryAsync(int id);
        Task<List<ClockEntry>> GetAllClockEntriesAsync();
    }
}
