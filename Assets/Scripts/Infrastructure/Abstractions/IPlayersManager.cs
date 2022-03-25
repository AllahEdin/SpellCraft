using System;
using System.Collections.Generic;

public interface IPlayersManager<T>
{
    event Action AllPlayersReady;

    event Action<IClientToServerConnection<T>> PlayerAdded;

    event Action<IClientToServerConnection<T>> PlayerRemoved;

    event Action MoveToDefaultPositions;

    /// <summary>
    /// Check if any empty slot left and associate connection to it 
    /// </summary>
    /// <returns>Slot id</returns>
    void ReserveSlot(IClientToServerConnection<T> connection, PlayerSlot slot);

    PlayerSlot GetEmptySlot();

    void SetPlayerReady(PlayerSlot player);

    /// <summary>
    /// Remove connection from slot
    /// </summary>
    /// <returns>Slot id</returns>
    void ClearConnectionSlot(PlayerSlot slot);

    IClientToServerConnection<T> GetPlayer(Guid id);

    IEnumerable<IClientToServerConnection<T>> GetPlayers();

    void MovePlayersToDefaultPositions();
}