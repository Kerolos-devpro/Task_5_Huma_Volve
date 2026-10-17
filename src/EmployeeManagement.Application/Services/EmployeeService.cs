using System.Diagnostics;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Mappings;
using EmployeeManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);
        return employees.Select(EmployeeMapper.ToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : EmployeeMapper.ToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var entity = EmployeeMapper.ToEntity(dto);
        var created = await _repository.AddAsync(entity, cancellationToken);

        _logger.LogInformation("Created employee with Id = {Id}", created.Id);
        return EmployeeMapper.ToDto(created);
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return null;

        EmployeeMapper.UpdateEntity(existing, dto);
        var updated = await _repository.UpdateAsync(existing, cancellationToken);

        _logger.LogInformation("Updated employee with Id = {Id}", id);
        return updated is null ? null : EmployeeMapper.ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);

        if (deleted)
            _logger.LogInformation("Deleted employee with Id = {Id}", id);

        return deleted;
    }

    public async Task<PerformanceCompareResultDto> CompareSequentialVsParallelAsync(
        CancellationToken cancellationToken = default)
    {
        // ---- Sequential ----
        var sw = Stopwatch.StartNew();

        var allEmployees = await _repository.GetAllAsync(cancellationToken);
        var itEmployees = await _repository.GetByDepartmentAsync("IT", cancellationToken);

        sw.Stop();
        var sequentialMs = sw.ElapsedMilliseconds;

        // ---- Parallel ----
        sw.Restart();

        var task1 = _repository.GetAllAsync(cancellationToken);
        var task2 = _repository.GetByDepartmentAsync("IT", cancellationToken);

        await Task.WhenAll(task1, task2);

        sw.Stop();
        var parallelMs = sw.ElapsedMilliseconds;

        var difference = sequentialMs - parallelMs;
        var improvement = sequentialMs > 0
            ? Math.Round((1 - (double)parallelMs / sequentialMs) * 100, 2)
            : 0;

        _logger.LogInformation(
            "Performance: Sequential = {Seq} ms, Parallel = {Par} ms, Improvement = {Imp}%",
            sequentialMs, parallelMs, improvement);

        return new PerformanceCompareResultDto
        {
            SequentialMs = sequentialMs,
            ParallelMs = parallelMs,
            DifferenceMs = difference,
            ImprovementPercentage = improvement,
            Conclusion = parallelMs < sequentialMs
                ? "Parallel execution improved performance."
                : "Parallel execution did not improve performance (or the difference is negligible with small data)."
        };
    }
}
