using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.Clock
{
    public class ClockManager : IClockManager
    {
        private readonly IClockEngine _engine;

        public ClockManager(IClockEngine engine)
        {
            _engine = engine;
        }

        public async Task<ClockEntryDTO> ClockInAsync(int userId, int shiftId)
        {
            var entry = await _engine.ClockInAsync(userId, shiftId);
            return ToDto(entry);
        }

        public async Task<ClockEntryDTO> ClockOutAsync(int userId)
        {
            var entry = await _engine.ClockOutAsync(userId);
            return ToDto(entry);
        }

        public async Task<List<ClockEntryDTO>> GetClockEntriesForUserAsync(int userId)
        {
            var entries = await _engine.GetClockEntriesForUserAsync(userId);
            return entries.Select(ToDto).ToList();
        }

        public async Task<List<ClockEntryDTO>> GetClockEntriesForShiftAsync(int shiftId)
        {
            var entries = await _engine.GetClockEntriesForShiftAsync(shiftId);
            return entries.Select(ToDto).ToList();
        }

        public async Task<List<ClockEntryDTO>> GetAllClockEntriesAsync()
        {
            var entries = await _engine.GetAllClockEntriesAsync();
            return entries.Select(ToDto).ToList();
        }

        public async Task<List<string>> GetClockedInUserNamesAsync()
        {
            return await _engine.GetClockedInUserNamesAsync();
        }

        public async Task<List<StatDataDTO>> GetHoursWorkedAsync(int userId, string range, DateTime? start)
        {
            return await _engine.GetHoursWorkedAsync(userId, range, start);
        }

        public async Task<List<StatDataDTO>> GetOvertimeHoursAsync(int userId, string range, DateTime? start)
        {
            return await _engine.GetOvertimeHoursAsync(userId, range, start);
        }

        public async Task<List<StatDataDTO>> GetLateClockInsAsync(int userId, string range, DateTime? start)
        {
            return await _engine.GetLateClockInsAsync(userId, range, start);
        }

        private ClockEntryDTO ToDto(ClockEntry entry)
        {
            return new ClockEntryDTO
            {
                Id = entry.Id,
                UserId = entry.UserId,
                ShiftId = entry.ShiftId,
                ClockInTime = entry.ClockIn,
                ClockOutTime = entry.ClockOut
            };
        }
    }
}
