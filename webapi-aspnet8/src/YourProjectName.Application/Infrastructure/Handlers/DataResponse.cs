namespace YourProjectName.Application.Infrastructure.Handlers;

public abstract record DataResponse<TData>() : IResponse where TData : notnull
{
    public TData Data { get; init; }

    public static TResponse Create<TResponse>(TData data) where TResponse : DataResponse<TData>, new()
    {
        return new TResponse
        {
            Data = data
        };
    }
}
