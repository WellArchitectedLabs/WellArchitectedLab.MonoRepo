using MasterData.Domain.AggregateModel.Cities;

namespace MasterData.Infrastructure.Repositories.Stubs;

/// <summary>
/// A city record like stored in Db
/// </summary>
/// <param name="Id">Maps to <see cref="City.Id"/></param>
/// <param name="Name">Maps to <see cref="City.Name"/></param>
/// <param name="Latitude">Maps to <see cref="City.Coordinates"/></param>
/// <param name="Longitude">Maps to <see cref="City.Coordinates"/></param>
internal record CityDbStub(int Id, string Name, decimal Latitude, decimal Longitude);