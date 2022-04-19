using System;
using System.Collections.Generic;
using Assets.Scripts;

/// <summary>
/// Сущность Player
/// Добавление/отключение и
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPlayersManager<T>
{
    event Action AllPlayersReady;

    event Action PlayersOnDefaultPositions;
    
    void SrvReserveSlot(IClientToServerConnection<T> connection, PlayerSlot slot);

    PlayerSlot SrvGetEmptySlot();

    void SrvSetPlayerReady(PlayerSlot slot, Player player);

    void SrvClearConnectionSlot(PlayerSlot slot);

    IClientToServerConnection<T> SrvGetPlayer(Guid id);

    IEnumerable<IClientToServerConnection<T>> SrvGetPlayers();

    void SrvMovePlayersToDefaultPositions();
}