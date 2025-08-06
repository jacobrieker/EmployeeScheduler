namespace EmployeeScheduler.Server.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string? ProfilePictureUrl { get; set; }
    }
}
