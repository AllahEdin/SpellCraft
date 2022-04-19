using System;

public interface ISpawnPointsManager
{
    void SrvAddSpawnPointsToPlayers();

    void SrvRemoveSpawnPointsToPlayers();

    SpawnPoint SrvGetSpawnPoint(Guid id);

    bool SrvIsEmpty(Guid id);

    void SrvSetIsEmpty(PlayerSlot playerSlot, Guid id, bool isEmpty);
}
