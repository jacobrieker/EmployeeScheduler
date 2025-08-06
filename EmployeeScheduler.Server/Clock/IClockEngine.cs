using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.Clock
{
    public interface IClockEngine
    {
        Task<ClockEntry> ClockInAsync(int userId, int shiftId);
        Task<ClockEntry> ClockOutAsync(int userId);
        Task<List<ClockEntry>> GetClockEntriesForUserAsync(int userId);
        Task<List<ClockEntry>> GetClockEntriesForShiftAsync(int shiftId);
        Task<List<ClockEntry>> GetAllClockEntriesAsync();
        Task<List<string>> GetClockedInUserNamesAsync();
        Task<List<StatDataDTO>> GetHoursWorkedAsync(int userId, string range, DateTime? start);
        Task<List<StatDataDTO>> GetOvertimeHoursAsync(int userId, string range, DateTime? start);
        Task<List<StatDataDTO>> GetLateClockInsAsync(int userId, string range, DateTime? start);

    }
}
