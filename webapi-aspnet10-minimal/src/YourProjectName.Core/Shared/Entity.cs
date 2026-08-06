using System.Text.Json.Serialization;

namespace YourProjectName.Core.Shared;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [JsonIgnore]
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
