using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EmployeeScheduler.Server.Clock;
using EmployeeScheduler.Server.DTOs;
using System.Security.Claims;

namespace EmployeeScheduler.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClockController : ControllerBase
    {
        private readonly IClockManager _manager;

        public ClockController(IClockManager manager)
        {
            _manager = manager;
        }

        [HttpPost("in/{shiftId}")]
        public async Task<ActionResult<ClockEntryDTO>> ClockIn(int shiftId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _manager.ClockInAsync(userId.Value, shiftId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("out")]
        public async Task<ActionResult<ClockEntryDTO>> ClockOut()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _manager.ClockOutAsync(userId.Value);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ClockEntryDTO>>> GetClockEntriesForUser(int userId)
        {
            var result = await _manager.GetClockEntriesForUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("shift/{shiftId}")]
        public async Task<ActionResult<List<ClockEntryDTO>>> GetClockEntriesForShift(int shiftId)
        {
            var result = await _manager.GetClockEntriesForShiftAsync(shiftId);
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin,owner")]
        public async Task<ActionResult<List<ClockEntryDTO>>> GetAllClockEntries()
        {
            var result = await _manager.GetAllClockEntriesAsync();
            return Ok(result);
        }


        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> IsUserClockedIn(int userId)
        {
            var entries = await _manager.GetClockEntriesForUserAsync(userId);
            var latest = entries
                .OrderByDescending(e => e.ClockInTime)
                .FirstOrDefault();

            bool isClockedIn = latest != null && latest.ClockOutTime == null;
            return Ok(isClockedIn);
        }

        private int? GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return idClaim != null && int.TryParse(idClaim.Value, out var userId) ? userId : (int?)null;
        }

        [HttpGet("active-users")]
        public async Task<IActionResult> GetClockedInUsers()
        {
            var result = await _manager.GetClockedInUserNamesAsync();
            return Ok(result);
        }

        [HttpGet("hours-worked/{userId}")]
        public async Task<IActionResult> GetHoursWorked(int userId, [FromQuery] string range, [FromQuery] DateTime? start)
        {
            var result = await _manager.GetHoursWorkedAsync(userId, range, start);
            return Ok(result);
        }

        [HttpGet("overtime-hours/{userId}")]
        public async Task<IActionResult> GetOvertimeHours(int userId, [FromQuery] string range, [FromQuery] DateTime? start)
        {
            var result = await _manager.GetOvertimeHoursAsync(userId, range, start);
            return Ok(result);
        }

        [HttpGet("clocked-in-late/{userId}")]

        public async Task<IActionResult> GetClockedInLate(int userId, [FromQuery] string range, [FromQuery] DateTime? start)
        {
            var result = await _manager.GetLateClockInsAsync(userId, range, start);
            return Ok(result);
        }


    }
}
