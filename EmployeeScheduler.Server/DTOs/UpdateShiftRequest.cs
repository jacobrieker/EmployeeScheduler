namespace EmployeeScheduler.Server.DTOs
{
    public class UpdateShiftRequest
    {
        public int ShiftId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
