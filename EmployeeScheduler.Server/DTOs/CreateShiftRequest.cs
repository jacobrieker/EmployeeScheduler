namespace EmployeeScheduler.Server.DTOs
{
    public class CreateShiftRequest
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
