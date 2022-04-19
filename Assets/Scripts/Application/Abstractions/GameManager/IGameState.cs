using System;

public interface IGameState
{
    event Action<IGameState> CompleteState;

    GameState State { get; }

    void StartState();
}