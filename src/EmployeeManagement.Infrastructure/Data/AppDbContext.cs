using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Department).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Salary).HasPrecision(18, 2);
        });

        // Seed data
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Name = "Ahmed Hassan",
                Email = "ahmed.hassan@company.com",
                Department = "IT",
                Salary = 15000,
                HireDate = new DateTime(2022, 3, 15)
            },
            new Employee
            {
                Id = 2,
                Name = "Sara Mohamed",
                Email = "sara.mohamed@company.com",
                Department = "HR",
                Salary = 12000,
                HireDate = new DateTime(2021, 7, 20)
            },
            new Employee
            {
                Id = 3,
                Name = "Omar Ali",
                Email = "omar.ali@company.com",
                Department = "Finance",
                Salary = 18000,
                HireDate = new DateTime(2020, 11, 5)
            }
        );
    }
}
