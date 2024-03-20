using Microsoft.EntityFrameworkCore;
using NexGen.Models;

namespace FYP_Management_System.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Supervisor> Supervisors { get; set; }
        public virtual DbSet<FYP> FYPs { get; set; }
    }
}
