using System;

public interface IGameStateTransition
{
    event Action<IGameStateTransition, GameState> OnComplete;

    GameState? From { get; }

    GameState To { get; set; }

    void StartTransit();
}