using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities;

public sealed class Project : AuditableEntity
{
    private Project()
    {
        Key = string.Empty;
        Name = string.Empty;
    }

    private Project(string name, string key, string? description)
        : base(Guid.NewGuid())
    {
        Name = NormalizeRequired(name, "Project name is required.");
        Key = NormalizeKey(key);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public string Key { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public bool IsArchived { get; private set; }

    public List<ProjectTask> Tasks { get; private set; } = [];

    public static Project Create(string name, string key, string? description)
    {
        return new Project(name, key, description);
    }

    public ProjectTask AddTask(
        string title,
        string? description,
        DateOnly? dueDate,
        ProjectTaskPriority priority)
    {
        if (IsArchived)
        {
            throw new DomainException("Archived projects cannot receive new tasks.");
        }

        var task = ProjectTask.Create(Id, title, description, dueDate, priority);
        Tasks.Add(task);

        return task;
    }

    public void Archive()
    {
        if (Tasks.Any(task => task.Status == ProjectTaskStatus.Open))
        {
            throw new DomainException("Projects with open tasks cannot be archived.");
        }

        IsArchived = true;
    }

    private static string NormalizeRequired(string value, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(errorMessage);
        }

        return value.Trim();
    }

    private static string NormalizeKey(string key)
    {
        var normalized = NormalizeRequired(key, "Project key is required.").ToUpperInvariant();
        if (normalized.Length is < 2 or > 12 || normalized.Any(character => !char.IsLetterOrDigit(character)))
        {
            throw new DomainException("Project key must be 2 to 12 alphanumeric characters.");
        }

        return normalized;
    }
}
