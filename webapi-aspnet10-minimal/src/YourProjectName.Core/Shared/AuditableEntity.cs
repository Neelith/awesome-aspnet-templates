namespace YourProjectName.Core.Shared;

public abstract class AuditableEntity : Entity
{
    public bool Deleted { get; private set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    public void MarkAsDeleted()
    {
        Deleted = true;
    }
}
