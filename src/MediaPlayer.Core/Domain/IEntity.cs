namespace MediaPlayer.Core.Domain;

/// <summary>
/// Marker contract for any aggregate root persisted via <c>IRepository&lt;T&gt;</c>.
/// Lets the generic repository find an entity's primary key without reflection.
/// </summary>
public interface IEntity
{
    Guid Id { get; }
}
