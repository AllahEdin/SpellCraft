using System;

public interface IGameManager
{
    event Action<GameState> SrvStateChanged;
}