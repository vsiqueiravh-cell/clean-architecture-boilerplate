using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }

    DbSet<ProjectTask> ProjectTasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
