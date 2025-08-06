using EmployeeScheduler.Server.DTOs;

namespace EmployeeScheduler.Server.ShiftManagement
{
    public interface IShiftManager
    {
        Task<ShiftResponse> CreateShiftAsync(CreateShiftRequest request);
        Task<List<ShiftResponse>> GetShiftsForUserAsync(int userId);
        Task<List<ShiftResponse>> GetShiftsForUserInRangeAsync(int userId, DateTime from, DateTime to);
        Task<ShiftResponse?> UpdateShiftAsync(UpdateShiftRequest request);
        Task<bool> DeleteShiftAsync(int shiftId);
        Task<List<ShiftResponse>> GetAllShiftsAsync();

    }
}
