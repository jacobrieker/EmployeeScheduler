namespace EmployeeScheduler.Server.Models
{
    public class ClockEntry
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ShiftId { get; set; }

        public Shift Shift { get; set; }

        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
    }
}
