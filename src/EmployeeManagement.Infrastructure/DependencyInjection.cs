using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core - InMemory for easy testing
        // To use SQL Server, replace with:
        // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("EmployeeDb"));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}
