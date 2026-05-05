using MediaPlayer.Core.Domain;

namespace MediaPlayer.Core.Abstractions;

/// <summary>
/// Generic CRUD over an aggregate. Implementations decide where data lives
/// (IndexedDB on the kiosk browser today; could be a server tomorrow).
/// </summary>
public interface IRepository<T> where T : IEntity
{
    Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
