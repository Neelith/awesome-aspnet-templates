namespace YourProjectName.Application.Infrastructure.Handlers;

public interface IDataResponse<TData> 
    : IResponse 
    where TData : notnull
{
    public TData Data { get; init; }
}
