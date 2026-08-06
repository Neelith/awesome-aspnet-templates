using Hermes.Results;

namespace YourProjectName.Core.Abstractions.Caching;

public sealed class CacheFactoryException : Exception
{
    public IReadOnlyList<IError> Errors { get; }
    public Dictionary<string, string?>? Metadata { get; }

    public CacheFactoryException(IReadOnlyList<IError> errors, Dictionary<string, string?>? metadata)
        : base("Cache factory operation failed")
    {
        Errors = errors;
        Metadata = metadata;
    }
}
