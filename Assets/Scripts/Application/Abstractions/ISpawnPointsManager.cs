using System;

public interface ISpawnPointsManager
{
    void AddSpawnPointsToPlayers();

    void RemoveSpawnPointsToPlayers();

    SpawnPoint GetSpawnPoint(Guid id);

    bool IsEmpty(Guid id);

    void SetIsEmpty(PlayerSlot playerSlot, Guid id, bool isEmpty);
}