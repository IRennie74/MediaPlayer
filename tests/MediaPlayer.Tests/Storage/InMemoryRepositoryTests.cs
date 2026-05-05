using FluentAssertions;
using MediaPlayer.Core.Domain;

namespace MediaPlayer.Tests.Storage;

/// <summary>
/// Exercises every member of the <c>IRepository&lt;T&gt;</c> contract using a
/// representative entity (<see cref="Location"/>). The same suite would catch
/// regressions if applied to the real <c>IndexedDbRepository</c> in a bUnit
/// integration test (deferred — bUnit can't talk to a real IndexedDB).
/// </summary>
public sealed class InMemoryRepositoryTests
{
    private readonly InMemoryRepository<Location> repo = new();

    private static Location NewLocation(string name = "HQ") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
    };

    [Fact]
    public async Task FindAll_OnEmpty_ReturnsEmpty()
    {
        var all = await repo.FindAllAsync();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_ThenFindById_ReturnsSameEntity()
    {
        var location = NewLocation();
        await repo.CreateAsync(location);

        var found = await repo.FindByIdAsync(location.Id);

        found.Should().Be(location);
    }

    [Fact]
    public async Task Update_OverwritesExistingEntity()
    {
        var original = NewLocation("Old");
        await repo.CreateAsync(original);

        var updated = original with { Name = "New" };
        await repo.UpdateAsync(updated);

        var found = await repo.FindByIdAsync(original.Id);
        found!.Name.Should().Be("New");
    }

    [Fact]
    public async Task Delete_RemovesEntity()
    {
        var location = NewLocation();
        await repo.CreateAsync(location);

        await repo.DeleteAsync(location.Id);

        (await repo.FindByIdAsync(location.Id)).Should().BeNull();
        (await repo.FindAllAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task FindById_OnMissingKey_ReturnsNull()
    {
        var missing = await repo.FindByIdAsync(Guid.NewGuid());
        missing.Should().BeNull();
    }
}
