using System;

public interface ISpawnPointsManager
{
    void AddSpawnPointsToPlayers();

    void RemoveSpawnPointsToPlayers();

    SpawnPoint SrvGetSpawnPoint(Guid id);

    bool IsEmpty(Guid id);

    void SetIsEmpty(PlayerSlot playerSlot, Guid id, bool isEmpty);
}
