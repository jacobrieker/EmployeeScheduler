using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EmployeeScheduler.Server.DTOs;
using EmployeeScheduler.Server.ShiftManagement;

namespace EmployeeScheduler.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftManagementController : ControllerBase
    {
        private readonly IShiftManager _manager;

        public ShiftManagementController(IShiftManager manager)
        {
            _manager = manager;
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetShiftsForUser(int userId)
        {
            if (!IsCurrentUser(userId) && !IsAdmin())
                return Forbid();

            var shifts = await _manager.GetShiftsForUserAsync(userId);
            return Ok(shifts);
        }

        [HttpGet("user/{userId}/range")]
        [Authorize]
        public async Task<IActionResult> GetShiftsForUserInRange(int userId, DateTime from, DateTime to)
        {
            if (!IsCurrentUser(userId) && !IsAdmin())
                return Forbid();

            var shifts = await _manager.GetShiftsForUserInRangeAsync(userId, from, to);
            return Ok(shifts);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin,owner")]
        public async Task<IActionResult> GetAllShifts()
        {
            var allShifts = await _manager.GetAllShiftsAsync();
            return Ok(allShifts);
        }

        [HttpPost]
        [Authorize(Roles = "admin,owner")]
        public async Task<IActionResult> CreateShift(CreateShiftRequest request)
        {
            var shift = await _manager.CreateShiftAsync(request);
            return Ok(shift);
        }

        [HttpPut]
        [Authorize(Roles = "admin,owner")]
        public async Task<IActionResult> UpdateShift(UpdateShiftRequest request)
        {
            var result = await _manager.UpdateShiftAsync(request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{shiftId}")]
        [Authorize(Roles = "admin,owner")]
        public async Task<IActionResult> DeleteShift(int shiftId)
        {
            var success = await _manager.DeleteShiftAsync(shiftId);
            return success ? Ok() : NotFound();
        }
        private bool IsAdmin()
        {
            return User.IsInRole("admin") || User.IsInRole("owner");
        }

        private bool IsCurrentUser(int userId)
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claimId != null && int.TryParse(claimId, out int id) && id == userId;
        }
    }
}
