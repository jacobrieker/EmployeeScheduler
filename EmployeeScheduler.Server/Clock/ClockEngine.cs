using System.Globalization;
using System.Runtime.CompilerServices;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;
using EmployeeScheduler.Server.ShiftManagement;
using EmployeeScheduler.Server.UserAccount;

namespace EmployeeScheduler.Server.Clock
{
    public class ClockEngine : IClockEngine
    {
        private readonly IClockAccessor _accessor;
        private readonly IShiftEngine _shiftEngine;
        private readonly IUserAccountAccessor _userAccessor;

        public ClockEngine(IClockAccessor accessor, IShiftEngine shiftEngine, IUserAccountAccessor userAccessor)
        {
            _accessor = accessor;
            _shiftEngine = shiftEngine;
            _userAccessor = userAccessor;
        }

        public async Task<ClockEntry> ClockInAsync(int userId, int shiftId)
        {
            var shift = await _shiftEngine.GetShiftByIdAsync(shiftId);
            if (shift == null || shift.UserId != userId)
                throw new InvalidOperationException("Invalid shift.");

            var entries = await _accessor.GetClockEntriesForUserAsync(userId);
            var alreadyClockedIn = entries.Any(e => e.ShiftId == shiftId && e.ClockOut == null);
            if (alreadyClockedIn)
                throw new InvalidOperationException("Already clocked in for this shift.");

            var newEntry = new ClockEntry
            {
                UserId = userId,
                ShiftId = shiftId,
                ClockIn = DateTime.UtcNow
            };

            return await _accessor.CreateClockEntryAsync(newEntry);
        }
        public async Task<ClockEntry> ClockOutAsync(int userId)
        {
            var entries = await _accessor.GetClockEntriesForUserAsync(userId);
            var entry = entries.FirstOrDefault(e => e.ClockOut == null);

            if (entry == null)
                throw new InvalidOperationException("No active clock-in found.");

            entry.ClockOut = DateTime.UtcNow;

            return await _accessor.UpdateClockEntryAsync(entry)
                   ?? throw new Exception("Failed to update clock entry.");
        }
        public async Task<List<ClockEntry>> GetClockEntriesForUserAsync(int userId)
        {
            return await _accessor.GetClockEntriesForUserAsync(userId);
        }

        public async Task<List<ClockEntry>> GetClockEntriesForShiftAsync(int shiftId)
        {
            return await _accessor.GetClockEntriesForShiftAsync(shiftId);
        }

        public async Task<List<ClockEntry>> GetAllClockEntriesAsync()
        {
            return await _accessor.GetAllClockEntriesAsync();
        }

        public async Task<List<string>> GetClockedInUserNamesAsync()
        {
            var entries = await _accessor.GetAllClockEntriesAsync();

            var clockedInUserIds = entries
                .Where(e => e.ClockOut == null)
                .Select(e => e.UserId)
                .Distinct()
                .ToList();

            var users = await _userAccessor.GetUsersByIdsAsync(clockedInUserIds);
            return users.Select(u => u.Name).ToList();
        }

        public async Task<List<StatDataDTO>> GetHoursWorkedAsync(int userId, string range, DateTime? start)
        {
            var (rangeStart, rangeEnd) = GetRangeBounds(range, start);
            var entries = await _accessor.GetClockEntriesForUserAsync(userId);

            var grouped = GroupEntriesByRange(entries, rangeStart, rangeEnd, range);

            return grouped.Select(g => new StatDataDTO
            {
                Date = g.Key,
                Value = g.Sum(e => e.ClockOut.HasValue ? (e.ClockOut.Value - e.ClockIn).TotalHours : 0)
            }).ToList();
        }

        public async Task<List<StatDataDTO>> GetOvertimeHoursAsync(int userId, string range, DateTime? start)
        {
            var (rangeStart, rangeEnd) = GetRangeBounds(range, start);
            var entries = await _accessor.GetClockEntriesForUserAsync(userId);

            var filteredEntries = entries
                .Where(e => e.ClockIn >= rangeStart && e.ClockIn <= rangeEnd)
                .ToList();

            double totalOvertime = 0;

            var weeklyGroups = filteredEntries
                .GroupBy(e => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    e.ClockIn,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday));

            foreach (var week in weeklyGroups)
            {
                var weekHours = week.Sum(e =>
                    e.ClockOut.HasValue ? (e.ClockOut.Value - e.ClockIn).TotalHours : 0);

                if (weekHours > 40)
                    totalOvertime += (weekHours - 40);
            }

            return new List<StatDataDTO>
            {
                new StatDataDTO
                {
                    Date = rangeStart.ToString("yyyy-MM-dd"),
                    Value = totalOvertime
                }
            };

        }


        public async Task<List<StatDataDTO>> GetLateClockInsAsync(int userId, string range, DateTime? start)
        {
            var (rangeStart, rangeEnd) = GetRangeBounds(range, start);
            var entries = await _accessor.GetClockEntriesForUserAsync(userId);
            var shifts = await _shiftEngine.GetShiftsForUserAsync(userId);

            var entriesInRange = entries.Where(e => e.ClockIn >= rangeStart && e.ClockIn <= rangeEnd).ToList();

            var lateEntries = entriesInRange
                .Where(e =>
                {
                    var shift = shifts.FirstOrDefault(s => s.Id == e.ShiftId);
                    return shift != null && e.ClockIn > shift.StartTime;
                });

            var grouped = lateEntries.GroupBy(e =>
            {
                return range.ToLower() switch
                {
                    "week" => e.ClockIn.ToString("MMM d"),
                    "month" => e.ClockIn.ToString("MMM d"),
                    "year" => e.ClockIn.ToString("MMM"),
                    _ => "Unknown"
                };
            });

            return grouped.Select(g => new StatDataDTO
            {
                Date = g.Key,
                Value = g.Count()
            }).ToList();
        }

        private IEnumerable<IGrouping<string, ClockEntry>> GroupEntriesByRange(List<ClockEntry> entries, DateTime start, DateTime end, string range)
        {
            return entries
                .Where(e => e.ClockIn >= start && e.ClockIn <= end)
                .GroupBy(e =>
                {
                    return range.ToLower() switch
                    {
                        "week" => e.ClockIn.ToString("MMM d"),
                        "month" => e.ClockIn.ToString("MMM d"),
                        "year" => e.ClockIn.ToString("MMM"),
                        _ => "Unknown"
                    };
                });
        }

        private (DateTime start, DateTime end) GetRangeBounds(string range, DateTime? startOverride)
        {
            var refDate = startOverride ?? DateTime.UtcNow;

            return range.ToLower() switch
            {
                "week" => (
                    refDate.Date.AddDays(-(int)refDate.DayOfWeek),
                    refDate.Date.AddDays(7 - (int)refDate.DayOfWeek).AddSeconds(-1)
                ),
                "month" => (
                    new DateTime(refDate.Year, refDate.Month, 1),
                    new DateTime(refDate.Year, refDate.Month, DateTime.DaysInMonth(refDate.Year, refDate.Month)).AddDays(1).AddSeconds(-1)
                ),
                "year" => (
                    new DateTime(refDate.Year, 1, 1),
                    new DateTime(refDate.Year, 12, 31, 23, 59, 59)
                ),
                _ => throw new ArgumentException("Invalid range")
            };
        }

    }
}
