using MediaPlayer.Core.Abstractions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Tests.Storage;

/// <summary>
/// Test double for <see cref="IRepository{T}"/>. Used in unit tests to keep
/// services decoupled from IndexedDB. Also serves as the SUT for the
/// <see cref="RepositoryContractTests{T}"/> contract.
/// </summary>
internal sealed class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly Dictionary<Guid, T> rows = new();

    public Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<T>>(rows.Values.ToList());

    public Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(rows.TryGetValue(id, out var found) ? found : null);

    public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        rows[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        rows[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        rows.Remove(id);
        return Task.CompletedTask;
    }
}
