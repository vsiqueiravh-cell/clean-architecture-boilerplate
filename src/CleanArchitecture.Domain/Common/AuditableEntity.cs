namespace CleanArchitecture.Domain.Common;

public abstract class AuditableEntity : Entity
{
    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id)
        : base(id)
    {
    }

    public DateTimeOffset CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = "system";

    public DateTimeOffset? LastModifiedAt { get; private set; }

    public string? LastModifiedBy { get; private set; }

    public void MarkCreated(DateTimeOffset createdAt, string createdBy)
    {
        CreatedAt = createdAt;
        CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? "system" : createdBy;
    }

    public void MarkModified(DateTimeOffset modifiedAt, string modifiedBy)
    {
        LastModifiedAt = modifiedAt;
        LastModifiedBy = string.IsNullOrWhiteSpace(modifiedBy) ? "system" : modifiedBy;
    }
}
