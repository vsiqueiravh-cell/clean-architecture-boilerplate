using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Projects;

public sealed record ProjectTaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    DateOnly? DueDate,
    ProjectTaskPriority Priority,
    ProjectTaskStatus Status,
    DateTimeOffset? CompletedAt)
{
    public static ProjectTaskResponse FromEntity(ProjectTask task)
    {
        return new ProjectTaskResponse(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.DueDate,
            task.Priority,
            task.Status,
            task.CompletedAt);
    }
}
