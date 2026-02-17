namespace WeatherInsights.Collector.Domain.Ports.UnitOfWork;

/// <summary>
/// UoW pattern permits a close correlation in a transaction manner
/// But applied to application level code
/// Using this pattern, combined with scoped dependency injection, we would create a way to manage transaction in c# code level
/// Without having the restriction to merge data access code into the same repository method 
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Begins a unit of work
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BeginAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Commit the unit of work when everything is fine
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Rollback it when any error occurs
    /// Simply using a try catch mechanism
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackAsync(CancellationToken cancellationToken);
}