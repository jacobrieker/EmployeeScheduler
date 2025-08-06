namespace EmployeeScheduler.Server.DTOs
{
    public class ClockEntryDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShiftId { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
    }
}
