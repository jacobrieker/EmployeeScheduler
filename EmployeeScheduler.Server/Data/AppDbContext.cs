using Microsoft.EntityFrameworkCore;
using EmployeeScheduler.Server.Models;

namespace EmployeeScheduler.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Shift> Shifts => Set <Shift>();
        public DbSet<ClockEntry> ClockEntries => Set<ClockEntry>();
    }
}
