using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities;

public sealed class ProjectTask : AuditableEntity
{
    private ProjectTask()
    {
        Title = string.Empty;
    }

    private ProjectTask(
        Guid projectId,
        string title,
        string? description,
        DateOnly? dueDate,
        ProjectTaskPriority priority)
        : base(Guid.NewGuid())
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project id is required.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Task title is required.");
        }

        ProjectId = projectId;
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DueDate = dueDate;
        Priority = priority;
        Status = ProjectTaskStatus.Open;
    }

    public Guid ProjectId { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public ProjectTaskPriority Priority { get; private set; }

    public ProjectTaskStatus Status { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public static ProjectTask Create(
        Guid projectId,
        string title,
        string? description,
        DateOnly? dueDate,
        ProjectTaskPriority priority)
    {
        return new ProjectTask(projectId, title, description, dueDate, priority);
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (Status == ProjectTaskStatus.Completed)
        {
            return;
        }

        Status = ProjectTaskStatus.Completed;
        CompletedAt = completedAt;
    }
}
