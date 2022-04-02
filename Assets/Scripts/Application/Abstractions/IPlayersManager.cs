using System;
using System.Collections.Generic;

/// <summary>
/// Сущность Player
/// Добавление/отключение и
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPlayersManager<T>
{
    event Action AllPlayersReady;

    event Action<IClientToServerConnection<T>> PlayerAdded;

    event Action<IClientToServerConnection<T>> PlayerRemoved;

    event Action MoveToDefaultPositions;

    void ReserveSlot(IClientToServerConnection<T> connection, PlayerSlot slot);

    PlayerSlot GetEmptySlot();

    void SetPlayerReady(PlayerSlot player);

    void ClearConnectionSlot(PlayerSlot slot);

    IClientToServerConnection<T> GetPlayer(Guid id);

    IEnumerable<IClientToServerConnection<T>> GetPlayers();

    void MovePlayersToDefaultPositions();
}