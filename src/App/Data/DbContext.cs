using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Service> Services => Set<Service>();   
    public DbSet<Profession> Professions => Set<Profession>();
    public DbSet<Service> Service => Set<Service>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeService> EmployeeServices => Set<EmployeeService>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeService>()
            .HasKey(x => new { x.EmployeeId, x.ServiceId });
            
        modelBuilder.Entity<EmployeeService>()
            .HasOne(x => x.Employee)
            .WithMany(e => e.EmployeeServices)
            .HasForeignKey(x => x.EmployeeId);

        modelBuilder.Entity<EmployeeService>()
            .HasOne(x => x.Service)
            .WithMany(s => s.EmployeeServices)
            .HasForeignKey(x => x.ServiceId);

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => a.TicketNumber)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.EmployeeId, a.ScheduledAt })
            .IsUnique();

        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Phone)
            .IsUnique();
        modelBuilder.Entity<Schedule>()
            .HasIndex(s => new { s.EmployeeId, s.WorkDate })
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasMaxLength(20);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.TicketNumber)
            .HasMaxLength(32);
    }
}
