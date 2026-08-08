namespace YourProjectName.Domain.Shared;

public abstract class AuditableEntity : Entity
{
    public bool Deleted { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAtUtc { get; private set; }
    public string? UpdatedBy { get; private set; }

    public void MarkAsDeleted()
    {
        Deleted = true;
    }
}
