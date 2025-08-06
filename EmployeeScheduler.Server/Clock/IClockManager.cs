using EmployeeScheduler.Server.DTOs;

namespace EmployeeScheduler.Server.Clock
{
    public interface IClockManager
    {
        Task<ClockEntryDTO> ClockInAsync(int userId, int shiftId);
        Task<ClockEntryDTO> ClockOutAsync(int userId);
        Task<List<ClockEntryDTO>> GetClockEntriesForUserAsync(int userId);
        Task<List<ClockEntryDTO>> GetClockEntriesForShiftAsync(int shiftId);
        Task<List<ClockEntryDTO>> GetAllClockEntriesAsync();
        Task<List<string>> GetClockedInUserNamesAsync();
        Task<List<StatDataDTO>> GetHoursWorkedAsync(int userId, string range, DateTime? start);
        Task<List<StatDataDTO>> GetOvertimeHoursAsync(int userId, string range, DateTime? start);
        Task<List<StatDataDTO>> GetLateClockInsAsync(int userId, string range, DateTime? start);

    }
}
