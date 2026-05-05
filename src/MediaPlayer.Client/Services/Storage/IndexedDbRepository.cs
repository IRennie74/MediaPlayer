using MediaPlayer.Core.Abstractions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Client.Services.Storage;

/// <summary>
/// Generic IndexedDB-backed repository. One instance per entity type;
/// caller picks the object store name through the constructor.
/// </summary>
public sealed class IndexedDbRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly IndexedDbInterop interop;
    private readonly string storeName;

    public IndexedDbRepository(IndexedDbInterop interop, string storeName)
    {
        this.interop = interop;
        this.storeName = storeName;
    }

    public async Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await interop.GetAllAsync<T>(storeName, cancellationToken).ConfigureAwait(false);
        return rows;
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await interop.GetByIdAsync<T>(storeName, id.ToString(), cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await interop.PutAsync(storeName, entity.Id.ToString(), entity, cancellationToken).ConfigureAwait(false);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await interop.PutAsync(storeName, entity.Id.ToString(), entity, cancellationToken).ConfigureAwait(false);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await interop.RemoveAsync(storeName, id.ToString(), cancellationToken).ConfigureAwait(false);
    }
}
