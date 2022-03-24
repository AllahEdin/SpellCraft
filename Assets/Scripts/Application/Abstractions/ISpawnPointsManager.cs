using System;

public interface ISpawnPointsManager
{
    void AddSpawnPointsToPlayers();

    void RemoveSpawnPointsToPlayers();

    SpawnPoint GetSpawnPoint(PlayerSlot playerSlot, Guid id);
}