using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.ShiftManagement
{
    public class ShiftManager : IShiftManager
    {
        private readonly IShiftEngine _engine;

        public ShiftManager(IShiftEngine engine)
        {
            _engine = engine;
        }

        public async Task<ShiftResponse> CreateShiftAsync(CreateShiftRequest request)
        {
            var shift = await _engine.CreateShiftAsync(request.UserId, request.StartTime, request.EndTime);
            return ToResponse(shift);
        }

        public async Task<List<ShiftResponse>> GetShiftsForUserAsync(int userId)
        {
            var shifts = await _engine.GetShiftsForUserAsync(userId);
            return shifts.Select(ToResponse).ToList();
        }

        public async Task<List<ShiftResponse>> GetShiftsForUserInRangeAsync(int userId, DateTime from, DateTime to)
        {
            var shifts = await _engine.GetShiftsForUserInRangeAsync(userId, from, to);
            return shifts.Select(ToResponse).ToList();
        }

        public async Task<ShiftResponse?> UpdateShiftAsync(UpdateShiftRequest request)
        {
            var shift = new Shift
            {
                Id = request.ShiftId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            var updated = await _engine.UpdateShiftAsync(shift);
            return updated == null ? null : ToResponse(updated);
        }

        public async Task<bool> DeleteShiftAsync(int shiftId)
        {
            return await _engine.DeleteShiftAsync(shiftId);
        }

        public async Task<List<ShiftResponse>> GetAllShiftsAsync()
        {
            var allShifts = await _engine.GetAllShiftsAsync();
            return allShifts.Select(ToResponse).ToList();
        }


        private ShiftResponse ToResponse(Shift shift)
        {
            return new ShiftResponse
            {
                Id = shift.Id,
                UserId = shift.UserId,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime
            };
        }
    }
}
