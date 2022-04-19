using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class CustomGameManager : CustomNetworkBehaviourBase, IGameManager
{

    public event Action<GameState> SrvStateChanged;

    private GameState _currentState;
    private IGameState _activeGameState;

    private readonly IGameStateTransition[] _transitions = {
        new EmptyTransition()
    };

    private readonly Dictionary<GameState, Dictionary<StateResult, GameState>> _stateMachineCases= new Dictionary<GameState, Dictionary<StateResult, GameState>>()
    {
        {GameState.ChooseSpell, new Dictionary<StateResult, GameState>()
        {
            {StateResult.Success, GameState.Round},
        }},
        {GameState.Round, new Dictionary<StateResult, GameState>()
        {
            {StateResult.Success, GameState.ChooseSpell},
        }},
    };

    private IGameState[] _gameStates;

    private void Start()
    {
        _currentState = GameState.WaitingForConnections;
        PlayersManager.AllPlayersReady += PlayersManagerOnAllPlayersReady;
        _gameStates =
            new IGameState[]
            {
                FindObjectOfType<ChooseSpellState>(),
                FindObjectOfType<RoundState>()
            };
    }

    private void PlayersManagerOnAllPlayersReady()
    {
        SetState(GameState.ChooseSpell);
    }

    [ServerCallback]
    private void TransitionComplete(IGameStateTransition transition, GameState state)
    {
        Debug.Log($"Transition completed to state {state.ToString()}");

        transition.OnComplete -= TransitionComplete;
        _currentState = state;
        var newState = 
            _gameStates.First(f => f.State == state);
        newState.CompleteState += OnCompleteState;
        _activeGameState = newState;
        newState.StartState();
    }

    [ServerCallback]
    private void OnCompleteState(IGameState state)
    {
        Debug.Log($"State {state.ToString()} completed");
        state.CompleteState -= OnCompleteState;
        var nextState =
            _stateMachineCases.Single(f => f.Key == state.State).Value.Single(w => w.Key == StateResult.Success).Value;
        SetState(nextState);
    }

    [ServerCallback]
    public void SetState(GameState state)
    {
        SrvStateChanged?.Invoke(state);
        Debug.Log($"Starting state {state.ToString()}");
        var transition =
            _transitions.FirstOrDefault(f => f.From == _currentState && f.To == state) ??
            new EmptyTransition()
            {
                To = state
            };

        transition.OnComplete += TransitionComplete; 
        transition.StartTransit();
        Debug.Log($"Starting transition");
    }

}