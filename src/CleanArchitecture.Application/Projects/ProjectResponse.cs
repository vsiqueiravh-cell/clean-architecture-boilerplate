using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Projects;

public sealed record ProjectResponse(
    Guid Id,
    string Key,
    string Name,
    string? Description,
    bool IsArchived,
    int OpenTaskCount,
    int CompletedTaskCount)
{
    public static ProjectResponse FromEntity(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.Key,
            project.Name,
            project.Description,
            project.IsArchived,
            project.Tasks.Count(task => task.Status == Domain.Enums.ProjectTaskStatus.Open),
            project.Tasks.Count(task => task.Status == Domain.Enums.ProjectTaskStatus.Completed));
    }
}
