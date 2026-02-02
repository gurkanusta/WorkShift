using Microsoft.EntityFrameworkCore;
using WorkShift.Domain.Entities;

namespace WorkShift.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
    .HasIndex(x => x.Email)
    .IsUnique();


        modelBuilder.Entity<Employee>()
            .HasIndex(x => x.Email)
            .IsUnique();

        
        modelBuilder.Entity<Shift>()
            .Property(x => x.StartTime)
            .HasColumnType("time");

        modelBuilder.Entity<Shift>()
            .Property(x => x.EndTime)
            .HasColumnType("time");
    }
}
