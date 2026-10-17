using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Mappings;

public static class EmployeeMapper
{
    public static EmployeeDto ToDto(Employee entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        Department = entity.Department,
        Salary = entity.Salary,
        HireDate = entity.HireDate
    };

    public static Employee ToEntity(CreateEmployeeDto dto) => new()
    {
        Name = dto.Name,
        Email = dto.Email,
        Department = dto.Department,
        Salary = dto.Salary,
        HireDate = dto.HireDate
    };

    public static void UpdateEntity(Employee entity, UpdateEmployeeDto dto)
    {
        entity.Name = dto.Name;
        entity.Email = dto.Email;
        entity.Department = dto.Department;
        entity.Salary = dto.Salary;
        entity.HireDate = dto.HireDate;
    }
}
