using System;

public class EmptyTransition : IGameStateTransition
{
    public event Action<IGameStateTransition, GameState> OnComplete;
    public GameState? From { get; }
    public GameState To { get; set; }

    public void StartTransit()
    {
        OnComplete?.Invoke(this, To);
    }
}